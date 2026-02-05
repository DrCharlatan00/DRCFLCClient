internal class Exceptions
{
    
     public class NoAvaibleConnectToServer : System.Exception
    {
        public NoAvaibleConnectToServer() { }
        public NoAvaibleConnectToServer(strisng message) : base(message) { }
        public NoAvaibleConnectToServer(string message, System.Exception inner) : base(message, inner) { }
        protected NoAvaibleConnectToServer(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class NullListAvaibleFlacMusic : System.Exception
    {
        public NullListAvaibleFlacMusic() {}
        public NullListAvaibleFlacMusic(string message) : base(message) {}
        public NullListAvaibleFlacMusic(string message, System.Exception inner) : base(message, inner) {}
        public NullListAvaibleFlacMusic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }


}