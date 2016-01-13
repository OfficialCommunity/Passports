using System;

namespace OCC.Passports.Common.Domains
{
    public sealed class PassportLevel : IComparable<PassportLevel>
    {
        private static readonly object CurrentPassportLevelLock = new object();
        private static readonly object CurrentPassportSessionLock = new object();

        public static readonly PassportLevel Trace = new PassportLevel(Constants.PassportLevel.Trace, 0);
        public static readonly PassportLevel Debug = new PassportLevel(Constants.PassportLevel.Debug, 1);
        public static readonly PassportLevel Info = new PassportLevel(Constants.PassportLevel.Info, 2);
        public static readonly PassportLevel Warn = new PassportLevel(Constants.PassportLevel.Warn, 3);
        public static readonly PassportLevel Error = new PassportLevel(Constants.PassportLevel.Error, 4);
        public static readonly PassportLevel Fatal = new PassportLevel(Constants.PassportLevel.Fatal, 5);
        public static readonly PassportLevel Exception = new PassportLevel(Constants.PassportLevel.Exception, 6);

        public static PassportLevel _current;
        public static string _currentSession;

        static PassportLevel ()
        {
            Current = Error;
        }

        public static PassportLevel Current
        {
            get
            {
                lock (CurrentPassportLevelLock)
                {
                    return _current;
                }
            }

            set
            {
                lock (CurrentPassportLevelLock)
                {
                    _current = value;
                }
            }

        }

        public static string Session
        {
            get
            {
                lock (CurrentPassportSessionLock)
                {
                    return _currentSession;
                }
            }

            set
            {
                lock (CurrentPassportSessionLock)
                {
                    _currentSession = value;
                }
            }

        }

        private readonly int _ordinal;
        private readonly string _name;

        private PassportLevel(string name, int ordinal)
        {
            _name = name;
            _ordinal = ordinal;
        }

        public string Name
        {
            get { return _name; }
        }

        internal int Ordinal
        {
            get { return _ordinal; }
        }

        internal static PassportLevel MaxLevel
        {
            get { return Exception; }
        }

        internal static PassportLevel MinLevel
        {
            get { return Trace; }
        }

        public static bool operator ==(PassportLevel level1, PassportLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return false;
            }

            return level1.Ordinal == level2.Ordinal;
        }

        public static bool operator !=(PassportLevel level1, PassportLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return !ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return true;
            }

            return level1.Ordinal != level2.Ordinal;
        }

        public static bool operator >(PassportLevel level1, PassportLevel level2)
        {
            return level1.Ordinal > level2.Ordinal;
        }

        public static bool operator <(PassportLevel level1, PassportLevel level2)
        {
            return level1.Ordinal < level2.Ordinal;
        }

        public static bool operator >=(PassportLevel level1, PassportLevel level2)
        {
            return level1.Ordinal >= level2.Ordinal;
        }

        public static bool operator <=(PassportLevel level1, PassportLevel level2)
        {
            return level1.Ordinal <= level2.Ordinal;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PassportLevel;
            if ((object) other == null)
            {
                return false;
            }

            return Ordinal == other.Ordinal;
        }

        public int CompareTo(PassportLevel other)
        {
            return Ordinal - other.Ordinal;
        }

        public static PassportLevel FromOrdinal(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return Trace;
                case 1:
                    return Debug;
                case 2:
                    return Info;
                case 3:
                    return Warn;
                case 4:
                    return Error;
                case 5:
                    return Fatal;
                case 6:
                    return Exception;

                default:
                    return null;
            }
        }

        public static PassportLevel FromString(string levelName)
        {
            if (levelName == null)
            {
                return null;
            }

            if (levelName.Equals(Constants.PassportLevel.Trace, StringComparison.OrdinalIgnoreCase))
            {
                return Trace;
            }

            if (levelName.Equals(Constants.PassportLevel.Debug, StringComparison.OrdinalIgnoreCase))
            {
                return Debug;
            }

            if (levelName.Equals(Constants.PassportLevel.Info, StringComparison.OrdinalIgnoreCase))
            {
                return Info;
            }

            if (levelName.Equals(Constants.PassportLevel.Warn, StringComparison.OrdinalIgnoreCase))
            {
                return Warn;
            }

            if (levelName.Equals(Constants.PassportLevel.Error, StringComparison.OrdinalIgnoreCase))
            {
                return Error;
            }

            if (levelName.Equals(Constants.PassportLevel.Fatal, StringComparison.OrdinalIgnoreCase))
            {
                return Fatal;
            }

            if (levelName.Equals(Constants.PassportLevel.Exception, StringComparison.OrdinalIgnoreCase))
            {
                return Exception;
            }

            return null;
        }
    }
}
