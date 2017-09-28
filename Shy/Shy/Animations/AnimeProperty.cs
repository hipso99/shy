using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shy.Animations {
    public interface IAnimeProperty {
        RepeatBehavior repeat { get; set; }
        IEasingFunction easing { get; set; }
        bool registerAnimation { get; set; }
        Timeline getTimeLine(object el,Duration duration);
        IEnumerable<PropertyPath> Targets { get; set; }
    }

    public abstract class AnimeProperty<T> : IAnimeProperty {
        public bool useInitialValue { get; protected set; }

        public T to { get; set; }
        public T from { get; set; }
        public abstract RepeatBehavior repeat { get; set; }
        public abstract Timeline getTimeLine(object el,Duration duration);
        public abstract IEnumerable<PropertyPath> Targets { get; set; }
        public bool registerAnimation { get; set; }

        public abstract IEasingFunction easing { get; set; }

        public AnimeProperty(T value) {
            this.to = value;
        }
        public AnimeProperty() { }

        public AnimeProperty(Func<T> generator) {
            if (generator != null) {
                this.to = generator();
            }
        }

        public AnimeProperty(T[] arr) {
            if (arr != null) {
                this.from = arr[0];
                this.to = arr[1];
                useInitialValue = true;
            }
        }

        public void setTargets(params PropertyPath[] targets) {
            Targets = targets;
        }
    }

    public class DoubleAnimeProperty : AnimeProperty<Double> {
        public DoubleAnimeProperty() { }

        public DoubleAnimeProperty(Double value) : base (value) {}

        public DoubleAnimeProperty(Func<Double> generator) : base(generator) {}

        public DoubleAnimeProperty(Double[] arr) : base(arr) {}

        public override RepeatBehavior repeat { get; set; }
        public override IEasingFunction easing { get; set; }

        public override IEnumerable<PropertyPath> Targets { get; set; }


        public override Timeline getTimeLine(object el,Duration duration) {
            DoubleAnimation timeLine;
            if (useInitialValue)
                timeLine = new DoubleAnimation(from,to,duration);
            else
                timeLine = new DoubleAnimation(to,duration);

            timeLine.EasingFunction = easing;
            return timeLine;
        }

        public static implicit operator DoubleAnimeProperty(Double value) {
            return new DoubleAnimeProperty(value);    
        }

        public static implicit operator DoubleAnimeProperty(Func<Double> generator) {
            return new DoubleAnimeProperty(generator);
        }

        public static implicit operator DoubleAnimeProperty(Double[] arr) {
            return new DoubleAnimeProperty(arr);
        }
    }


    public class ThicknessAnimeProperty : AnimeProperty<Thickness> {
        public override IEasingFunction easing { get; set; }
        public override RepeatBehavior repeat { get; set; }
        public ThicknessAnimeProperty() { }
        public ThicknessAnimeProperty(Thickness value) : base (value) { }

        public ThicknessAnimeProperty(TranslateAnimeProperty transX,TranslateAnimeProperty transY) {
            to = getThicknessFromArr(transX.to, transY.to);
            from = getThicknessFromArr(transX.from,transY.from);
            this.useInitialValue = transX.useInitialValue || transY.useInitialValue;
        }

        public ThicknessAnimeProperty(Func<Thickness> generator) : base(generator) { }

        public ThicknessAnimeProperty(Double margin) {
            this.to = new Thickness(margin);
        }

        public ThicknessAnimeProperty(Double[] arr) {
            this.to = getThicknessFromArr(arr);
        }

        public ThicknessAnimeProperty(String marginStr) {
            String[] valuesStr = marginStr.Split(',');
            if (valuesStr.Length > 0) {
                double[] values = new double[valuesStr.Length];
                for (int i = 0 ; i < valuesStr.Length ; i++)
                    values[i] = double.Parse(valuesStr[i].Trim());
                this.to = getThicknessFromArr(values);
            }
        }

        public override IEnumerable<PropertyPath> Targets { get; set; } = new PropertyPath[] { new PropertyPath("Margin") };

        public override Timeline getTimeLine(object el,Duration duration) {

            ThicknessAnimation timeLine;
            var currentValue = (el as FrameworkElement).Margin;
            to = new Thickness(
                to.Left + currentValue.Left,
                to.Top + currentValue.Top,
                currentValue.Right - to.Left,
                currentValue.Bottom - to.Top
            );

            if (useInitialValue) {
                from  = new Thickness(
                    from.Left + currentValue.Left,
                    from.Top + currentValue.Top,
                    currentValue.Right - from.Left,
                    currentValue.Bottom - from.Top
                );
                timeLine = new ThicknessAnimation(from,to,duration);
            } else { 
                timeLine = new ThicknessAnimation(to,duration);
            }
            timeLine.EasingFunction = easing;
            return timeLine;
        }

        private Thickness getThicknessFromArr(params Double[] arr) {
            Thickness margin = default(Thickness);
            if (arr.Length == 2) {
                margin = new Thickness(arr[0],arr[1],arr[0],arr[1]);
            } else if (arr.Length == 4) {
                margin = new Thickness(arr[0],arr[1],arr[2],arr[3]);
            }
            return margin;
        }

        public static implicit operator ThicknessAnimeProperty(Double value) {
            return new ThicknessAnimeProperty(value);
        }

        public static implicit operator ThicknessAnimeProperty(Func<Thickness> generator) {
            return new ThicknessAnimeProperty(generator);
        }

        public static implicit operator ThicknessAnimeProperty(Double[] arr) {
            return new ThicknessAnimeProperty(arr);
        }

        public static implicit operator ThicknessAnimeProperty(String margin) {
            return new ThicknessAnimeProperty(margin);
        }
    }

    public class TranslateAnimeProperty : DoubleAnimeProperty {
        public Axis axis { get; set; }
        public override IEasingFunction easing { get; set; }
        public override RepeatBehavior repeat { get; set; }
        public override IEnumerable<PropertyPath> Targets { get; set; } = new PropertyPath[] { new PropertyPath("Margin") };
        public TranslateAnimeProperty() {
            //registerAnimation = true;
        }

        public TranslateAnimeProperty(Double value) : base(value) {
            //setTargets(new PropertyPath("Margin"));
        }

        public TranslateAnimeProperty(Func<double> generator) : base(generator) { }

        public TranslateAnimeProperty(Double[] arr) : base(arr) { }


        public override Timeline getTimeLine(object el,Duration duration) {
            ThicknessAnimation timeLine;
            double startOffset, endOffset;
            var currentValue = (el as FrameworkElement).Margin;
            if (axis == Axis.X) {
                startOffset = currentValue.Left + to;
                endOffset = currentValue.Right - to;
                var end = new Thickness(startOffset,currentValue.Top,endOffset,currentValue.Bottom);
                if (useInitialValue) {
                    startOffset = currentValue.Left + from;
                    endOffset = currentValue.Right - from;
                    var start = new Thickness(startOffset,currentValue.Top,endOffset,currentValue.Bottom);
                    timeLine = new ThicknessAnimation(start,end,duration);
                } else {
                    timeLine = new ThicknessAnimation(end,duration);
                }
            } else if (axis == Axis.Y) {
                startOffset = currentValue.Top + to;
                endOffset = currentValue.Bottom - to;
                var end = new Thickness(currentValue.Left,startOffset,currentValue.Left,endOffset);
                if (useInitialValue) {
                    startOffset = currentValue.Top + from;
                    endOffset = currentValue.Bottom - from;
                    var start = new Thickness(currentValue.Left,startOffset,currentValue.Left,endOffset);
                    timeLine = new ThicknessAnimation(start,end,duration);
                } else {
                    timeLine = new ThicknessAnimation(end,duration);
                }
            } else {
                timeLine = new ThicknessAnimation(currentValue,duration);
            }
            timeLine.EasingFunction = easing;
            return timeLine;
        }

        public static implicit operator TranslateAnimeProperty(Double value) {
            return new TranslateAnimeProperty(value);
        }

        public static implicit operator TranslateAnimeProperty(Func<double> generator) {
            return new TranslateAnimeProperty(generator);
        }

        public static implicit operator TranslateAnimeProperty(Double[] arr) {
            return new TranslateAnimeProperty(arr);
        }
    }

    public enum Axis { X, Y, Z }

}
