public class GradeCalculator
{

public static GradeResult CalculateGrade(int score)
    {
      return score switch 
      {
       >=70 => new GradeResult {Grade='A' ,GradeUnit = 5, Remark = "Excellent"},
       >=60 => new GradeResult {Grade='B' ,GradeUnit = 4, Remark = "Very Good"},
       >=50 => new GradeResult {Grade='C' ,GradeUnit = 3, Remark = "Good"},
       >=45 => new GradeResult {Grade='D' ,GradeUnit = 2, Remark = "Fair"},
       >=40 => new GradeResult {Grade='E' ,GradeUnit = 1, Remark = "Pass"},
       _    => new GradeResult {Grade='F' ,GradeUnit = 0, Remark = "Fail"},
      };
    
    }
}
