using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Organiser.Common.Converters
{
    public class ElementSizeMathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string equasion = parameter as string;
            double size = 0.0;
            if(equasion != null)
            {
                string[] numbers = equasion.Split('M');
                double biggerValue = System.Convert.ToDouble(value);
                string opperand = numbers[1];
                double smallerValue = System.Convert.ToDouble(numbers[2]);
                string secondOperand = "";
                double thirdVal = 0;      
                if(numbers.Length > 3)
                {
                    secondOperand = numbers[3];
                    thirdVal = System.Convert.ToDouble(numbers[4]);
                }
                switch (opperand)
                {
                    case "-":
                        size = biggerValue - smallerValue;    
                        break;

                    case "+":
                        size = biggerValue + smallerValue;
                        break;

                    case "*":
                        size = biggerValue * smallerValue;
                        break;

                    case "/":
                        size = biggerValue / smallerValue;
                        break;

                    default:
                        break;
                }

                switch (secondOperand)
                {
                    case "-":
                        size = size - thirdVal;
                        break;

                    case "+":
                        size = size + thirdVal;
                        break;

                    case "*":
                        size = size * thirdVal;
                        break;

                    case "/":
                        size = size / thirdVal;
                        break;

                    default:
                        break;
                }
            }

            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
