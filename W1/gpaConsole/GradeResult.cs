using System.ComponentModel.DataAnnotations;

public class GradeResult
{
    public char Grade {get; set;}
    public int GradeUnit {get; set;}

    public required string Remark {get; set;} 
}

