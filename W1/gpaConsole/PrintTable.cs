public class PrintTable
{
    public void Print(List<Course> courses)
    {
        Console.WriteLine();
        Console.WriteLine("ACADEMIC RESULTS");
        Console.WriteLine("--------------------------------------------------------------------------");

        Console.WriteLine(
            "{0,-18} {1,11} {2,7} {3,11} {4,11} {5,-12}",
            "COURSE & CODE",
            "COURSE UNIT",
            "GRADE",
            "GRADE-UNIT",
            "WEIGHT Pt.",
            "REMARK"
        );

        Console.WriteLine("--------------------------------------------------------------------------");

        foreach (Course course in courses)
        {
            Console.WriteLine(
                "{0,-18} {1,11} {2,7} {3,11} {4,11} {5,-12}",
                course.CourseCode,
                course.CourseUnit,
                course.Grade,
                course.GradeUnit,
                course.WeightPoint,
                course.Remark
            );
        }

        Console.WriteLine("---------------------------------------------------------------------------");
    }

    
}