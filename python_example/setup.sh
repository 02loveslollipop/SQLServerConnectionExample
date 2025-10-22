#!/usr/bin/env bash

# Secure, distro-aware setup for Microsoft SQL Server ODBC Driver 18 on Debian/Ubuntu
# - Removes deprecated apt-key usage
# - Adds Microsoft package repo via packages-microsoft-prod.deb
# - Installs msodbcsql18 (and optional mssql-tools18)
# - Idempotent and non-interactive

set -Eeuo pipefail

# Use sudo when not root
SUDO=""
if [[ ${EUID:-$(id -u)} -ne 0 ]]; then
	SUDO="sudo"
fi

export DEBIAN_FRONTEND=noninteractive

# Basic prerequisites
$SUDO apt-get update -y
$SUDO apt-get install -y curl ca-certificates apt-transport-https gnupg lsb-release

# Detect distro/version
if [[ -r /etc/os-release ]]; then
	# shellcheck disable=SC1091
	. /etc/os-release
else
	echo "Cannot detect OS. /etc/os-release not found." >&2
	exit 1
fi

ID=${ID:-}
VERSION_ID=${VERSION_ID:-}

# Figure out the correct packages-microsoft-prod.deb URL
PKG_DEB_URL=""
case "$ID" in
	ubuntu)
		case "$VERSION_ID" in
			24.04) PKG_DEB_URL="https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb" ;;
			22.04) PKG_DEB_URL="https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb" ;;
			20.04) PKG_DEB_URL="https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb" ;;
			18.04) PKG_DEB_URL="https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb" ;;
			*)    PKG_DEB_URL="https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb" ;;
		esac
		;;
	debian)
		case "$VERSION_ID" in
			12|12.*) PKG_DEB_URL="https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb" ;;
			11|11.*) PKG_DEB_URL="https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb" ;;
			10|10.*) PKG_DEB_URL="https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb" ;;
			*)       PKG_DEB_URL="https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb" ;;
		esac
		;;
	*)
		echo "Unsupported distro: $ID $VERSION_ID" >&2
		exit 1
		;;
esac

# Add Microsoft repo idempotently
if ! dpkg -s packages-microsoft-prod >/dev/null 2>&1; then
	tmpdeb="$(mktemp /tmp/packages-microsoft-prod.XXXXXX.deb)"
	trap 'rm -f "$tmpdeb"' EXIT
	curl -fsSL "$PKG_DEB_URL" -o "$tmpdeb"
	$SUDO dpkg -i "$tmpdeb"
	rm -f "$tmpdeb"
	trap - EXIT
fi

$SUDO apt-get update -y

# Install the SQL Server ODBC Driver 18
$SUDO ACCEPT_EULA=Y apt-get install -y msodbcsql18

# Optional: command-line tools (sqlcmd, bcp). Enable with INSTALL_MSSQL_TOOLS18=1
if [[ "${INSTALL_MSSQL_TOOLS18:-0}" == "1" ]]; then
	$SUDO ACCEPT_EULA=Y apt-get install -y mssql-tools18
	# Add to PATH for future shells
	if ! grep -qs "/opt/mssql-tools18/bin" "$HOME/.bashrc"; then
		echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> "$HOME/.bashrc"
	fi
fi

# Optional but helpful for building pyodbc from source
if [[ "${INSTALL_UNIXODBC_DEV:-1}" == "1" ]]; then
	$SUDO apt-get install -y unixodbc-dev
fi

echo "Microsoft ODBC Driver 18 for SQL Server installed successfully."