using System;
using Scribble.Application.Versioning;

namespace Scribble.Application;

public struct Version
{
	public ReleaseType ReleaseType { get; set; }
	public uint Release { get; set; }
	public uint Major { get; set; }
	public uint Minor { get; set; }

	public Version()
	{
		ReleaseType = ReleaseType.Unknown;
		Release = 0;
		Major = 0;
		Minor = 0;
	}

	public Version(ReleaseType releaseType, uint release, uint major, uint minor)
	{
		ReleaseType = releaseType;
		Release = release;
		Major = major;
		Minor = minor;
	}

	public override string ToString()
	{
		if (ReleaseType is ReleaseType.Unknown)
			return "Unknown";
		else
			return $"{(ReleaseType is ReleaseType.Release ? "" : $"{ReleaseType}_")}{Release}.{Major}.{Minor}";
	}

	public override bool Equals(object obj) =>
		obj is Version version && this == version;

	public override int GetHashCode() =>
		HashCode.Combine(ReleaseType, Release, Major, Minor);

	public static Version Parse(string version)
	{
		ReleaseType releaseType = ReleaseType.Release;
		uint release = 0;
		uint major = 0;
		uint minor = 0;

		string[] versionParts = version.Split('_');
		if (versionParts.Length > 2)
			throw new ArgumentException("Invalid version format");
		else if (versionParts.Length > 1)
		{
			releaseType = Enum.Parse<ReleaseType>(versionParts[0]);
			version = versionParts[1];
		}

		versionParts = version.Split('.');
		if (versionParts.Length != 3)
			throw new ArgumentException("Invalid version format");

		release = uint.Parse(versionParts[0]);
		major = uint.Parse(versionParts[1]);
		minor = uint.Parse(versionParts[2]);

		return new(releaseType, release, major, minor);
	}

	public static bool TryParse(string version, out Version result)
	{
		try
		{
			result = Parse(version);
			return true;
		}
		catch
		{
			result = new();
			return false;
		}
	}

	public static bool operator >(Version v1, Version v2) =>
		v1.ReleaseType > v2.ReleaseType ||
		v1.Release > v2.Release ||
		v1.Major > v2.Major ||
		v1.Minor > v2.Minor;

	public static bool operator <(Version v1, Version v2) =>
		v1.ReleaseType < v2.ReleaseType ||
		v1.Release < v2.Release ||
		v1.Major < v2.Major ||
		v1.Minor < v2.Minor;

	public static bool operator >=(Version v1, Version v2) =>
		v1 == v2 || v1 > v2;

	public static bool operator <=(Version v1, Version v2) =>
		v1 == v2 || v1 < v2;

	public static bool operator ==(Version v1, Version v2) =>
		v1.ReleaseType == v2.ReleaseType &&
		v1.Release == v2.Release &&
		v1.Major == v2.Major &&
		v1.Minor == v2.Minor;

	public static bool operator !=(Version v1, Version v2) =>
		v1.ReleaseType != v2.ReleaseType ||
		v1.Release != v2.Release ||
		v1.Major != v2.Major ||
		v1.Minor != v2.Minor;
}
