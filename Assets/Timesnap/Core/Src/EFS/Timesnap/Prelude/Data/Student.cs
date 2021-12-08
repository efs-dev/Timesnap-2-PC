using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LitJson;

namespace EFS.Timesnap.Data
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly]
    public class Student
    {
        public int id;
        public string school_specific_id;
        public string name;
        public int classroom;

        public override string ToString()
        {
            return JsonMapper.ToJson(this);
        }
    }
}