using System;

namespace DareWare.SWGoH.Core
{
    public struct AllyCode : IEquatable<AllyCode>, IComparable<AllyCode>
    {
        #region Constructors
        public AllyCode( uint ac ) : this( ac, true ) { }
        public AllyCode( string ac ) : this( uint.Parse( ac ), true ) { }
        public AllyCode( uint ac, bool validate )
        {
            if ( validate ) ThrowInvalid( ac );
            _value = ac;
        }
        #endregion

        #region The Property / Field
        private readonly uint _value;
        public uint Value => _value;
        #endregion

        #region Methods
        public bool IsValid() => IsValid( this.Value );
        #endregion

        #region Object Overrides
        public override string ToString() => this.Value.ToString().Insert( 6, "-" ).Insert( 3, "-" );
        public override bool Equals( object obj ) => ( obj is AllyCode ac ) ? Equals( ac ) : base.Equals( obj );
        public override int GetHashCode() => this.Value.GetHashCode();
        #endregion

        #region IEquatable
        public bool Equals( AllyCode ac ) => this.Value.Equals( ac.Value );
        #endregion

        #region IComparable
        public int CompareTo( AllyCode ac ) => this.Value.CompareTo( ac.Value );
        #endregion

        #region Conversation Operators
        public static implicit operator bool( AllyCode ac ) => ac.IsValid();
        public static implicit operator uint( AllyCode ac ) => ac.Value;
        public static implicit operator string( AllyCode ac ) => ac.Value.ToString();
        public static implicit operator AllyCode( string s ) => new AllyCode( s );
        public static implicit operator AllyCode( uint n ) => new AllyCode( n );
        #endregion

        #region Comparison Operators
        public static bool operator ==( AllyCode left, AllyCode right ) => left.Equals( right );
        public static bool operator !=( AllyCode left, AllyCode right ) => !left.Equals( right );
        public static bool operator >( AllyCode left, AllyCode right ) => left.CompareTo( right ) >= 1;
        public static bool operator <( AllyCode left, AllyCode right ) => left.CompareTo( right ) <= -1;
        public static bool operator >=( AllyCode left, AllyCode right ) => left.CompareTo( right ) >= 0;
        public static bool operator <=( AllyCode left, AllyCode right ) => left.CompareTo( right ) <= 0;
        #endregion

        #region Public Static Values
        public static AllyCode None { get; } = new AllyCode( 0, false );
        public static AllyCode MaxValue { get; } = new AllyCode( _max, false );
        public static AllyCode MinValue { get; } = new AllyCode( _min, false );
        #endregion

        #region Private Static Helpers
        private static void ThrowInvalid( uint ac ) { if ( !IsValid( ac ) ) throw new ArgumentOutOfRangeException(); }
        private static bool IsValid( uint ac ) => ( ( ac >= _min ) && ( ac <= _max ) );
        private static uint _max = 999_999_999U;
        private static uint _min = 100_000_000U;
        #endregion
    }
}
