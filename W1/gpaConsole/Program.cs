List<Course> courses = new List<Course>();
GradeCalculator gradeCalculator = new GradeCalculator();

bool continueAdding = true;

while(continueAdding){
Console.WriteLine("Enter Course Code: ");
string courseCode = Console.ReadLine();

Console.WriteLine("Enter Course Unit From 1 to 5: ");
int courseUnit = int.Parse(Console.ReadLine());
if(courseUnit < 0 || courseUnit > 5)
        {
           throw new Exception("Invalid input, Course Unit must be between 0 to 5");
        }

Console.WriteLine("Enter in score 0-100: ");
int score = int.Parse(Console.ReadLine());

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
courses.Add(completedCourse);

Console.WriteLine("Would you like to enter in another course?: ");
string response = Console.ReadLine();

if(response == "yes" || response == "y")
{
    continueAdding = true;
}else
{
    continueAdding = false;
}

PrintTable printer = new PrintTable();
printer.Print(courses);
}

