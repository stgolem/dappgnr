using System.Runtime.Serialization;

namespace AutoGen.I
{
    class SObject : ISerializable
    {
        #region Implementation of ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(SObject));
        }

        public SObject(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion
    }
}
