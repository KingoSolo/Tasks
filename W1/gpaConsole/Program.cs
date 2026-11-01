
List<Course> courses = new List<Course>();
GradeCalculator gradeCalculator = new GradeCalculator();

Console.WriteLine("Enter Course Code: ");
string courseCode = Console.ReadLine();

Console.WriteLine("Enter Course Unit: ");
int  courseUnit = int.Parse(Console.ReadLine());

Console.WriteLine("Enter in score 0-100: ");
int score = int.Parse(Console.ReadLine());

Course course = new Course
{
    CourseCode = courseCode,
    CourseUnit = courseUnit,
    Score = score
};

GradeResult result = gradeCalculator.CalculateGrade(score);

Course completedCourse = new Course
{
    CourseCode = courseCode,
    CourseUnit = courseUnit,

    Grade = result.Grade,
    GradeUnit = result.GradeUnit,
    Remark = result.Remark,

    WeightPoint = courseUnit * result.GradeUnit
};
