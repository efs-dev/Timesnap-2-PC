using EFS.Timesnap.Data;

public class StudentPickList : PickList<Student>
{
    public void Awake()
    {
        Stringifier = student => student.name;
    }
}