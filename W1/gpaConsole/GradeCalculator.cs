public class GradeCalculator
{

public GradeResult CalculateGrade(int score)
    {
        if(score < 0 || score > 100)
        {
           throw new Exception("Invalid input, Score must be between 0 to 100");
        }
        if(score >= 70 && score <= 100)
        {
            return new GradeResult
            {
                Grade = 'A',
                GradeUnit = 5,
                Remark = "Excellent"
            };
        }else if(score >= 60 && score <= 69)
        {
            return new GradeResult
            {
                Grade = 'B',
                GradeUnit = 4,
                Remark = "Very Good"
            };
        }else if(score >= 50 && score <= 59)
        {
            return new GradeResult
            {
                Grade = 'C',
                GradeUnit = 3,
                Remark = "Good"
            };
        }else if(score >= 45 && score <= 49)
        {
            return new GradeResult
            {
                Grade = 'D',
                GradeUnit = 2,
                Remark = "Fair"
            };
        }else if(score >= 40 && score <= 44)
        {
            return new GradeResult
            {
                Grade = 'E',
                GradeUnit = 1,
                Remark = "Pass"
            };
        }else 
        {
            return new GradeResult
            {
                Grade = 'F',
                GradeUnit = 0,
                Remark = "Fail"
            };
        }
    
    }
}
