namespace TIKSN.Versioning
{
	public class Version : System.IComparable<Version>, System.IEquatable<Version>
	{
		private const Milestone DefaultMilestone = Milestone.GA;
		private const int DefaultPrereleaseNumber = -1;

		private Milestone milestone;
		private int prereleaseNumber;
		private System.Version release;
		private System.DateTime? releaseDate;

		public Version(int ReleaseMajor, int ReleaseMinor)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(System.Version Release)
		{
			this.release = Release;
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(System.Version Release, System.DateTime ReleaseDate)
		{
			this.release = Release;
			this.milestone = DefaultMilestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(System.Version Release, Milestone Milestone)
		{
			this.release = Release;
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(System.Version Release, Milestone Milestone, System.DateTime ReleaseDate)
		{
			this.release = Release;
			this.milestone = Milestone;
			this.prereleaseNumber = DefaultPrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, int PrereleaseNumber)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, int PrereleaseNumber, System.DateTime ReleaseDate)
		{
			this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber)
		{
			this.release = Release;
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = null;
		}

		public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber, System.DateTime ReleaseDate)
		{
			this.release = Release;
			this.milestone = Milestone;
			this.PrereleaseNumber = PrereleaseNumber;
			this.releaseDate = ReleaseDate;
		}

		//public Version(string ReleaseVersion)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = DefaultMilestone;
		//    this.prereleaseNumber = DefaultPrereleaseNumber;
		//    this.releaseDate = null;
		//}

		//public Version(string ReleaseVersion, System.DateTime ReleaseDate)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = DefaultMilestone;
		//    this.prereleaseNumber = DefaultPrereleaseNumber;
		//    this.releaseDate = ReleaseDate;
		//}

		//public Version(string ReleaseVersion, Milestone Milestone)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = Milestone;
		//    this.prereleaseNumber = DefaultPrereleaseNumber;
		//    this.releaseDate = null;
		//}

		//public Version(string ReleaseVersion, Milestone Milestone, System.DateTime ReleaseDate)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = Milestone;
		//    this.prereleaseNumber = DefaultPrereleaseNumber;
		//    this.releaseDate = ReleaseDate;
		//}

		//public Version(string ReleaseVersion, Milestone Milestone, int PrereleaseNumber)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = Milestone;
		//    this.PrereleaseNumber = PrereleaseNumber;
		//    this.releaseDate = null;
		//}

		//public Version(string ReleaseVersion, Milestone Milestone, int PrereleaseNumber, System.DateTime ReleaseDate)
		//{
		//    this.release = new System.Version(ReleaseVersion);
		//    this.milestone = Milestone;
		//    this.PrereleaseNumber = PrereleaseNumber;
		//    this.releaseDate = ReleaseDate;
		//}

		public Milestone Milestone
		{
			get
			{
				return this.milestone;
			}
		}

		public int PrereleaseNumber
		{
			get
			{
				return this.prereleaseNumber;
			}
			private set
			{
				if (value < -1)
				{
					throw new System.ArgumentOutOfRangeException("PrereleaseNumber");
				}
				else
				{
					this.prereleaseNumber = value;
				}
			}
		}

		public System.Version Release
		{
			get
			{
				return this.release;
			}
		}

		public System.DateTime? ReleaseDate
		{
			get
			{
				return this.releaseDate;
			}
		}

		public Stability Stability
		{
			get
			{
				if (this.milestone >= Milestone.RTM)
				{
					return Versioning.Stability.Stable;
				}
				else
				{
					return Versioning.Stability.Unstable;
				}
			}
		}

		public static bool operator !=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) != 0;
		}

		public static bool operator <(Version v1, Version v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator <=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) <= 0;
		}

		public static bool operator ==(Version v1, Version v2)
		{
			return v1.CompareTo(v2) == 0;
		}

		public static bool operator >(Version v1, Version v2)
		{
			return v1.CompareTo(v2) > 0;
		}

		public static bool operator >=(Version v1, Version v2)
		{
			return v2.CompareTo(v2) >= 0;
		}

		public int CompareTo(Version that)
		{
			if (object.ReferenceEquals(this, that))
			{
				return 0;
			}
			else
			{
				int ReleaseComparison = this.release.CompareTo(that.release);

				if (ReleaseComparison == 0)
				{
					int MilestoneComparison = this.milestone - that.milestone;

					if (this.milestone > that.milestone)
					{
						return 1;
					}
					else if (this.milestone < that.milestone)
					{
						return -1;
					}
					else
					{
						System.Diagnostics.Debug.Assert(this.milestone == that.milestone);

						if (this.prereleaseNumber > that.prereleaseNumber)
						{
							return 1;
						}
						else if (this.prereleaseNumber < that.prereleaseNumber)
						{
							return -1;
						}
						else
						{
							System.Diagnostics.Debug.Assert(this.prereleaseNumber == that.prereleaseNumber);

							return 0;
						}
					}
				}
				else
				{
					return ReleaseComparison;
				}
			}
		}

		public bool Equals(Version that)
		{
			return this.CompareTo(that) == 0;
		}

		public string ToLongReleaseString()
		{
			return this.release.ToString();
		}

		public string ToPrereleaseString()
		{
			string MilestoneTag = string.Empty;

			switch (this.milestone)
			{
				case Milestone.PreAlpha:
					MilestoneTag = "pre-alpha";
					break;

				case Milestone.Alpha:
					MilestoneTag = "alpha";
					break;

				case Milestone.Beta:
					MilestoneTag = "beta";
					break;

				case Milestone.ReleaseCandidate:
					MilestoneTag = "rc";
					break;

				case Milestone.RTM:
					MilestoneTag = "rtm";
					break;

				case Versioning.Milestone.GA:
					MilestoneTag = "ga";
					break;

				default:
					throw new System.NotSupportedException("Unsupported milestone name.");
			}

			if (this.prereleaseNumber == DefaultPrereleaseNumber)
			{
				return MilestoneTag;
			}
			else
			{
				return string.Format("{0}{1}", MilestoneTag, this.prereleaseNumber);
			}
		}

		public string ToShortReleaseString()
		{
			if (this.release.Revision > 0)
			{
				return this.release.ToString(4);
			}
			else if (this.release.Build > 0)
			{
				return this.release.ToString(3);
			}
			else if (this.release.Minor > 0)
			{
				return this.release.ToString(2);
			}
			else
			{
				return this.release.ToString(1);
			}
		}

		public override string ToString()
		{
			if (this.prereleaseNumber == DefaultPrereleaseNumber && this.milestone == DefaultMilestone)
			{
				return this.ToShortReleaseString();
			}
			else
			{
				return string.Format("{0}-{1}", this.ToShortReleaseString(), this.ToPrereleaseString());
			}
		}
	}
}