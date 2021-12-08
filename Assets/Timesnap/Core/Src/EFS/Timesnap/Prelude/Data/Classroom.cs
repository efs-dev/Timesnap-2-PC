using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LitJson;

namespace EFS.Timesnap.Data
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly]
    public class Classroom
    {
        public int id;
        public string classroom_name;
        public string class_code;
        public string grade_level;
        public int school;
        public string teacher_name;
        
        public override string ToString()
        {
            return JsonMapper.ToJson(this);
        }
    }
}