using System;
using System.Linq;
List<Course> courses = new List<Course>();

bool continueAdding = true;

while(continueAdding){
    Console.WriteLine("Enter Course Code: ");
    string? courseCode = Console.ReadLine();

    int courseUnit;
    while(true)
    {   
        Console.WriteLine("Enter Course Unit from 1 to 5");
        if(int.TryParse(Console.ReadLine(), out courseUnit) && courseUnit is >= 1 and <= 5)
        {
          break;
        }
        Console.WriteLine("Invalid!, Please Enter in the course unit from 1 to 5\n");
    }

    int score;
    while(true)
    {
        Console.WriteLine("Enter in score 0-100: ");
        if(int.TryParse(Console.ReadLine(), out score) && score is >= 0 and <= 100)
        {
            break;
        }
        Console.WriteLine("Invalid score! Please enter a number from 0 to 100.\n");
    };

    GradeResult result = GradeCalculator.CalculateGrade(score);

    Course completedCourse = new Course
    {
        CourseCode = courseCode ?? string.Empty,
        CourseUnit = courseUnit,

        Grade = result.Grade,
        GradeUnit = result.GradeUnit,
        Remark = result.Remark,

        WeightPoint = courseUnit * result.GradeUnit
    };
    courses.Add(completedCourse);

    int totalCourseUnits = courses.Sum<Course>(c => c.CourseUnit);
    int totalCourseUnitsPassed = 0;
    if(courseUnit > 1)
    {
         totalCourseUnitsPassed = +courseUnit;
    }
    int totalWeightPoints = courses.Sum<Course>(c => c.WeightPoint);
    double Gpa = (double)totalWeightPoints/totalCourseUnits;

    Console.WriteLine("Would you like to enter in another course?: ");
    string? response = Console.ReadLine();

    if(response == "yes" || response == "y")
    {
        continueAdding = true;
    }else
    {
        continueAdding = false;
        PrintTable printer = new PrintTable();
        printer.Print(courses);
        Console.WriteLine($"Total Course Unit Registered is {totalCourseUnits}");
        Console.WriteLine($"Total Course Unit Passed is {totalCourseUnitsPassed}");
        Console.WriteLine($"Total Weight Point is {totalWeightPoints}");
        Console.WriteLine($"Your GPA is {Gpa:N2} to 2 decimal places");
        
    }

}

