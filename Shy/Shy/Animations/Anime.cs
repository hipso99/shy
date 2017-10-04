using Shy.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shy.Animations {
    ///<author>Alejandro Cárdenas</author>
    /// <summary>
    /// A class to create animations with ease.
    /// </summary>
    public class Anime {

        public const int TIME = 250;

        public bool isRunning { get; private set; }
        public bool isPaused { get; private set; }

        private int currentAnimation;
        private List<Storyboard> animations;
        private FrameworkElement[] targets;
        private Action<Object> animationsCompleted;
        private Action animationsChanged;


        public Anime(AnimeProperties properties) {
            animations = new List<Storyboard>();
            targets = properties.getTargets();
            animations.Add(createStoryboard(properties));
        }

        public Anime then(AnimeProperties properties) {
            properties.targets = this.targets;
            animations.Add(createStoryboard(properties));
            return this;
        }

        public Anime completed(Action<Object> completed) {
            this.animationsCompleted = completed;
            return this;
        }

        public Anime changed(Action changed) {
            this.animationsChanged = changed;
            return this;
        }

        public void start() {
            if (!isRunning) {
                currentAnimation = 0;
                isRunning = true;
                animations[currentAnimation].Begin(targets.ElementAt(0),true);
            } else if (isPaused) {
                animations[currentAnimation].Resume(targets.ElementAt(0));
                isPaused = false;
            }
        }

        public void pause() {
            if (isRunning) {
                animations[currentAnimation].Pause(targets.ElementAt(0));
                isPaused = true;
            }
        }

        private void animationCompleted(object sender,EventArgs e) {
            currentAnimation++;
            if (currentAnimation < animations.Count) {
                animations[currentAnimation].Begin(targets.ElementAt(0),true);
            } else {
                isRunning = false;
                animationsCompleted?.Invoke(sender);
            }
        }


        private Storyboard createStoryboard(IEnumerable<FrameworkElement> elements,int time,IEnumerable<Tuple<String,object>> properties) {
            Storyboard sb = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(time));

            foreach (var el in elements) {
                foreach (var prop in properties) {
                    Timeline animation = createAnimation(duration, prop.Item2);
                    Storyboard.SetTargetProperty(animation,new PropertyPath(prop.Item1));
                    Storyboard.SetTarget(animation,el);
                    sb.Children.Add(animation);
                }
            }

            sb.Completed += animationCompleted;
            sb.Changed += (s,e) => {
                animationsChanged?.Invoke();
            };
            return sb;
        }

        private Storyboard createStoryboard(AnimeProperties properties) {
            Storyboard sb = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(properties.time));

            foreach (var el in properties.getTargets()) {
                foreach (var prop in properties.getAnimeProperties()) {
                    if (properties.easing != null && prop.easing == null) {
                        prop.easing = properties.easing;
                    }

                    Timeline animation = prop.getTimeLine(el,duration);

                    //if(properties.repeat != null)
                    //    animation.RepeatBehavior = properties.repeat;

                    foreach (var target in prop.Targets) {
                        Storyboard.SetTargetProperty(animation,target);
                    }
                    Storyboard.SetTarget(animation,el);
                    
                    sb.Children.Add(animation);
                }
            }
            if(properties.delay > 0 )
                sb.BeginTime = TimeSpan.FromMilliseconds(properties.delay);
            sb.Completed += animationCompleted;
            sb.Changed += (s,e) => {
                animationsChanged?.Invoke();
            };
            return sb;
        }

        public Timeline createAnimation(Duration duration,object value) {
            Timeline tl = null;

            if (value is byte) {
                tl = new ByteAnimation((byte)value,duration);
            } else if (value is Int16) {
                tl = new Int16Animation((Int16)value,duration);
            } else if (value is Int32) {
                tl = new Int32Animation((Int32)value,duration);
            } else if (value is Int64) {
                tl = new Int64Animation((Int64)value,duration);
            } else if (value is double) {
                tl = new DoubleAnimation((double)value,duration);
            } else if (value is decimal) {
                tl = new DecimalAnimation((decimal)value,duration);
            } else if (value is Thickness) {
                tl = new ThicknessAnimation((Thickness)value,duration);
            } else if (value is SolidColorBrush) {
                tl = new ColorAnimation((value as SolidColorBrush).Color, duration);
            }

            return tl;
        }

    }


    public class AnimeProperties {

        private Dictionary<String,object> parameters = new Dictionary<string, object>();
        private Dictionary<String,IAnimeProperty> animeProperties = new Dictionary<string,IAnimeProperty>();


        public FrameworkElement target { get; set; }

        public FrameworkElement[] targets { get; set; }

        public  RepeatBehavior repeat { get; set; }

        public int time { get; set; } = Anime.TIME;

        public bool loop { get; set; } = false;

        public IEasingFunction easing { get; set; }

        public int delay { get; set; }

        public DoubleAnimeProperty height {
            get {
                if (animeProperties.ContainsKey("Height"))
                    return (DoubleAnimeProperty)animeProperties["Height"];
                return 0;
            }
            set {
                if (value != null) {
                    value.setTargets(new PropertyPath("Height"));
                }
                animeProperties["Height"] = value;
            }
        }

        public DoubleAnimeProperty width {
            get {
                if (animeProperties.ContainsKey("Width"))
                    return (DoubleAnimeProperty)animeProperties["Width"];
                return 0;
            }
            set {
                if (value != null) {
                    value.setTargets(new PropertyPath("Width"));
                }
                animeProperties["Width"] = value;
            }
        }

        //public DoubleAnimeProperty scale {
        //    get {
        //        if (animeProperties.ContainsKey("scale"))
        //            return (DoubleAnimeProperty)animeProperties["scale"];
        //        return 0;
        //    }
        //    set {
        //        if (value != null) {
        //            value.setTargets(new PropertyPath("Width"));
        //        }
        //        animeProperties["scale"] = value;
        //    }
        //}

        public TranslateAnimeProperty translateX {
            get {
                if (animeProperties.ContainsKey("translateX"))
                    return (TranslateAnimeProperty)animeProperties["translateX"];
                return 0;
            }
            set {
                if (value != null) {
                    value.axis = Axis.X;
                }
                animeProperties["translateX"] = value;
            }
        }

        public TranslateAnimeProperty translateY {
            get {
                if (animeProperties.ContainsKey("translateY"))
                    return (TranslateAnimeProperty)animeProperties["translateY"];
                return 0;
            }
            set {
                if (value != null) {
                    value.axis = Axis.Y;
                }
                animeProperties["translateY"] = value;
            }
        }

        public ThicknessAnimeProperty margin { get; set; }

        public double opacity {
            get {
                if (parameters.ContainsKey("Opacity"))
                    return (double)parameters["Opacity"];
                return 0;
            }
            set {
                parameters["Opacity"] = value;
            }
        }

        public List<Tuple<string,object>> getProperties() {
            List<Tuple<string,object>> par = new List<Tuple<string,object>>();
            foreach (var el in parameters) {
                par.Add(new Tuple<string, object>(el.Key,el.Value));
            }
            return par;
        }

        public IEnumerable<IAnimeProperty> getAnimeProperties() {
            if (animeProperties.ContainsKey("translateX") && animeProperties.ContainsKey("translateY")) {
                animeProperties["margin"] = new ThicknessAnimeProperty((TranslateAnimeProperty)animeProperties["translateX"],(TranslateAnimeProperty)animeProperties["translateY"]);
                animeProperties.Remove("translateX");
                animeProperties.Remove("translateY");
            }

            foreach (var prop in animeProperties) {
                yield return prop.Value;
            }
        }

        //public IEnumerable<FrameworkElement> getTargets() {
        public FrameworkElement[] getTargets() {
            if (targets == null) {
                return new FrameworkElement[] { target };
            } else /*if (targets != null)*/ {
                return targets;
                //foreach (FrameworkElement el in targets) {
                //    yield return el;
                //}
            }
        }
    }

}
