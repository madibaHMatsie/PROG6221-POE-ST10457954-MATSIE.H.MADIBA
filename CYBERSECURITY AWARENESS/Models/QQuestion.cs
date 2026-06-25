using System;
using System.Collections.Generic;
using System.Text;

namespace CYBERSECURITY_AWARENESS.Models
{
    public class QQuestion
    {
    public string Question { get; set; }
	public List<string> Options { get; set; }
	public int CorrectAnswerIndex { get; set; }
	public string Explanation { get; set; }
	public string Topic { get; set; }

}
}
