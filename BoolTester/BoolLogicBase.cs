using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoolTester
{
    public class BoolLogicBase
    {
        public static evalResult evaluateValue(string exp, inputObj[] inputs)
        {
            string s_exp = exp;
            var elements_tmp = Regex.Matches(exp, "~[a-z0-1]|[a-z0-1]");
            var elements = new List<string>();
            var result = new evalResult();
            foreach (var el in elements_tmp)
            {
                elements.Add(el.ToString());
            }
            for (int i = 0; i < elements.Count; i++)
            {
                for (int i2 = 0; i2 < inputs.Length; i2++)
                {
                    elements[i] = elements[i].Replace(inputs[i2].letter + "", inputs[i2].state.ToString());
                    s_exp = s_exp.Replace(inputs[i2].letter + "", inputs[i2].state.ToString());
                }
                var regex = new Regex("[a-z]");
                var newText = regex.Replace(elements[i], "0");
                elements[i] = newText;
                newText = regex.Replace(s_exp, "0");
                s_exp = newText;
            }
            if (s_exp.Length < 1) s_exp = "0";
            var prev_s_exp = "";
            result.addStrStep(s_exp);
            while (s_exp.Contains("+") || s_exp.Contains("*") || s_exp.Contains("~"))
            {
                if (s_exp.Length == 1) return result;
                s_exp = boolUtils.mergeAll(s_exp, result);
                if (prev_s_exp == s_exp)
                {
                    result.addStrStep("ERROR");
                    return result;
                }
                prev_s_exp = s_exp;
                
            }
            
            return result;
        }
        public static evalResult isFunctionsEqualsHard(string exp1, string exp2)
        {
            var result = new evalResult();
            var vars1 = new List<char>();
            var vars2 = new List<char>();
            foreach (var item in exp1.ToCharArray())
            {
                if (Regex.IsMatch(item.ToString(), "[a-w]"))
                {
                    if (!vars1.Contains(item))
                        vars1.Add(item);
                }
            }
            foreach (var item in exp2.ToCharArray())
            {
                if (Regex.IsMatch(item.ToString(), "[a-w]"))
                {
                    if (!vars2.Contains(item))
                        vars2.Add(item);
                }
            }

            int max1 = Convert.ToInt32(Math.Pow((double)2, (double)vars1.Count));
            int max2 = Convert.ToInt32(Math.Pow((double)2, (double)vars2.Count));

            var results1 = new List<string>();
            var results2 = new List<string>();

            for (int i = 0; i < max1; i++)
            {
                string binary = Convert.ToString(i, 2);
                while (binary.Length < vars1.Count) binary = "0" + binary;

                var inputs = new List<inputObj>();

                for (int i2 = 0; i2 < vars1.Count; i2++)
                {
                    inputs.Add(new inputObj(binary[i2] == '0' ? 0 : 1, vars1[i2]));
                }

                var res = evaluateValue(exp1, inputs.ToArray());
                results1.Add(res.result_str());
                var res2 = evaluateValue(exp2, inputs.ToArray());
                results2.Add(res2.result_str());

                result.addStrStep($"X: {binary} > {res.result_str()}    Y: {binary} > {res2.result_str()}       > {(res.result_str()==res2.result_str()?"1":"0")}");
            }

            for (int i = 0; i < max2; i++)
            {
                string binary = Convert.ToString(i, 2);
                while (binary.Length < vars2.Count) binary = "0" + binary;

                var inputs = new List<inputObj>();

                for (int i2 = 0; i2 < vars2.Count; i2++)
                {
                    inputs.Add(new inputObj(binary[i2] == '0' ? 0 : 1, vars2[i2]));
                }
                var res = evaluateValue(exp1, inputs.ToArray());
                results1.Add(res.result_str());
                var res2 = evaluateValue(exp2, inputs.ToArray());
                results2.Add(res2.result_str());

                result.addStrStep($"X: {binary} > {res.result_str()}    Y: {binary} > {res2.result_str()}       > {(res.result_str() == res2.result_str() ? "1" : "0")}");
            }
            result.addStrStep("\n");

            var equals = true;
            for (int i = 0; i < results1.Count; i++)
            {
                result.addStrStep($"{results1[i]}*{results1[i]} = {(results1[i] != results2[i] ? "0" : "1")}");
                if (results1[i] != results2[i])
                {
                    equals = false;
                }
            }
            result.addStrStep("\n");
            result.addStrStep($"{(equals?"1":"0")}");
            return result;
        }
    }
    public class inputObj
    {
        public int state = 0;
        public char letter = 'a';
        public inputObj(int state, char letter)
        {
            this.state = state;
            this.letter = letter;
        }
    }
    public class evalResult
    {
        public List<List<string>> steps = new List<List<string>>();
        public List<string> steps_str = new List<string>();
        public void addStep(string[] el)
        {
            steps.Add(el.ToList());
        }
        public void addStrStep(string step)
        {
            steps_str.Add(step);
        }
        public List<string> result()
        {
            if (steps.Count < 1)
            {
                return null;
            }
            return steps[steps.Count - 1];
        }
        public string result_str()
        {
            if (steps_str.Count < 1)
            {
                return null;
            }
            return steps_str[steps_str.Count - 1];
        }
    }
    class boolUtils
    {
        public static string mergeAdd(string exp, evalResult result)
        {
            if (exp.Contains("+"))
            {
                exp = ReplaceFirst(exp, "0+1", "1");
                exp = ReplaceFirst(exp, "1+1", "1");
                exp = ReplaceFirst(exp, "1+0", "1");
                exp = ReplaceFirst(exp, "0+0", "0");
                result.addStrStep(exp);
            }

            return exp;
        }
        public static string mergeMult(string exp, evalResult result)
        {
            while (exp.Contains("00") || exp.Contains("01") || exp.Contains("10") || exp.Contains("11"))
            {
                exp = ReplaceFirst(exp, "00", "0*0");
                exp = ReplaceFirst(exp, "11", "1*1");
                exp = ReplaceFirst(exp, "10", "1*0");
                exp = ReplaceFirst(exp, "01", "0*1");

                result.addStrStep(exp);
            }
            if (exp.Contains("*"))
            {
                exp = ReplaceFirst(exp, "0*1", "0");
                exp = ReplaceFirst(exp, "1*1", "1");
                exp = ReplaceFirst(exp, "1*0", "0");
                exp = ReplaceFirst(exp, "0*0", "0");
                result.addStrStep(exp);
            }

            return exp;
        }
        public static string mergeBrackets(string exp, evalResult result)
        {
            if (exp.Contains("(0)") || exp.Contains("(1)"))
            {
                exp = exp.Replace("(0)", "0");
                exp = exp.Replace("(1)", "1");
                result.addStrStep(exp);
            }

            return exp;
        }
        public static string mergeNegations(string exp, evalResult result)
        {
            if (exp.Contains("~0") || exp.Contains("~1"))
            {
                exp = exp.Replace("~0", "1");
                exp = exp.Replace("~1", "0");
                result.addStrStep(exp);
            }

            return exp;
        }
        public static string mergeAll(string exp, evalResult result)
        {
            exp = mergeNegations(exp, result);

            exp = mergeMult(exp, result);
            exp = mergeAdd(exp, result);
            exp = mergeBrackets(exp, result);
            
            return exp;
        }
        public static string toFunctions(string exp)
        {
            exp = Regex.Replace(exp, @"([a-z0-1])", "($1)");
            string p_not = @"~([A-Z]{1,}{[^()+*]*})|~(\([A-Z]{1,}{[^()+*]*}\))|~([a-z0-1]{1,})|~\(([a-z0-1]{1,})\)|~\(([^()]*)\)";
            string p_not_s = @"~([a-z0-1])";
            string s_not = @"(NOT{$1$2$3$4$5})";
            string s_not_s = @"(NOT{$1})";
            string p_or = @"(~?\(.{1,}\))\+(~?\(.{1,}\))";
            string s_or = @"OR($1,$2)";
            string p_and = @"(~?\(.{1,}\))\*(~?\(.{1,}\))";
            string s_and = @"AND($1,$2)";

            string p_br = @"\(\(([^()]{1,})\)\)";
            string p_br2 = @"\(([^()+*]{1,})\)";
            string s_br = @"($1)";
            string s_br2 = @"$1";
            var check2 = exp;
            while (exp.Contains("+") || exp.Contains("*") || exp.Contains("~"))
            {
                check2 = exp;
                var check = exp;
                while (exp.Contains("+") || exp.Contains("*") || exp.Contains("~"))
                {
                    check = exp;
                    while (exp.Contains("~"))
                    {
                        check = exp;
                        exp = Regex.Replace(exp, p_not_s, s_not_s);
                        if (check == exp) break;
                    }
                    while (exp.Contains("*"))
                    {
                        check = exp;
                        exp = ReplaceRegexFirst(exp, p_and, s_and);
                        if (check == exp) break;
                    }
                    while (exp.Contains("+")&&!exp.Contains("*"))
                    {
                        check = exp;
                        exp = ReplaceRegexFirst(exp, p_or, s_or);
                        if (check == exp) break;
                    }


                    if (check == exp) break;
                }
                while (exp.Contains("~"))
                {
                    check = exp;
                    exp = Regex.Replace(exp, p_not, s_not);
                    if (check == exp) break;
                }
                if (exp == check2) break;
            }
            while (exp.Contains("(") && exp.Contains(")"))
            {
                check2 = exp;
                //exp = Regex.Replace(exp, p_br, s_br);
                //exp = Regex.Replace(exp, p_br2, s_br2);
                if (check2 == exp) break;
            }
            return exp;
        }
        public static string ReplaceFirst(string input, string find, string replacement)
        {
            var regex = new Regex(Regex.Escape(find));
            var newText = regex.Replace(input, replacement, 1);
            return newText;
        }
        public static string ReplaceRegexFirst(string input, string regex, string replacement)
        {
            var regex2 = new Regex(regex);
            var newText = regex2.Replace(input, replacement, 1);
            return newText;
        }
        public static string PlusToOR(string input) {
            var chars = input.ToCharArray();
            int bracketCounter = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                var _char = chars[i];
                var pchar = i>0?chars[i - 1]:' ';
                var nchar = i+1<chars.Length?chars[i + 1]:' ';

                if (_char == '{') bracketCounter++;
                if (_char == '}') bracketCounter--;
                if (_char == '+')
                {
                    var brackets = getStringInBracket(input, bracketCounter);
                }
            }
            return input;

            
        }
        public static string[] getStringInBracket(string input,int bracketID)
        {
            var chars = input.ToCharArray();
            var output = new List<string>();
            var o = "";
            int i_bracketCounter = 0;
            var building = false;
            for (int i = 0; i < chars.Length; i++)
            {
                var _char = chars[i];
                var pchar = i > 0 ? chars[i - 1] : ' ';
                var nchar = i + 1 < chars.Length ? chars[i + 1] : ' ';
                
                if (_char == '}') i_bracketCounter--;
                if (i_bracketCounter == bracketID)
                {
                    building = true;
                }
                if (building && i_bracketCounter < bracketID)
                {
                    output.Add(o);
                    o = "";
                    building = false;
                }
                if (building)
                {
                    o += "" + _char;
                }
                if (_char == '{') i_bracketCounter++;
            }
            if (building) output.Add(o);
            return output.ToArray();
        }
    }
}
