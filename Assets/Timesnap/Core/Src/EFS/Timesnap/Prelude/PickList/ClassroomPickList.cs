using EFS.Timesnap.Data;

public class ClassroomPickList : PickList<Classroom>
{
    public void Awake()
    {
        Data.Add(new Classroom{classroom_name = "foo", teacher_name = "bla"});
        Data.Add(new Classroom{classroom_name = "bar", teacher_name = "boop"});
        Stringifier = classroom =>
        {
            var teacherName = classroom.teacher_name;
            var possessiveTeacherName = teacherName;
            if (teacherName.EndsWith("s"))
            {
                possessiveTeacherName += "'";
            }
            else
            {
                possessiveTeacherName += "'s";
            }

            return possessiveTeacherName + " " + classroom.classroom_name;
        };
    }
}