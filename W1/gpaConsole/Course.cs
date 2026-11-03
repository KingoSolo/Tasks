public class Course
{
   public string CourseCode {get; set;} = string.Empty;
   public int CourseUnit {get; set;}
   public int Score {get; set;}

   public char Grade {get; set;}
   public int GradeUnit {get; set;}
   public int WeightPoint{get; set;}
   public string Remark{get; set;} = string.Empty;
}