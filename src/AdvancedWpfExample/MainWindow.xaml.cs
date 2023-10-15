using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FosterScript.Examples.Modules;
using FosterScript.Core.Agents;
using FosterScript.Core.Worlds;
using System.Diagnostics;
using FosterScript.Examples.Modules.SmartModules;

namespace FosterScript.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UIElementCollection _children;
        private readonly Random _random = new();
        private readonly Dictionary<Actor, Ellipse> _actors = new();
        private readonly IndefiniteWorld _world = new(1000 / 60); // Set refreshrate to 60fps

        public MainWindow()
        {
            InitializeComponent();

            _children = ActorCanvas.Children;

            _world.StepDone += Tick;
            _world.ActorMoved += OnActorMoved;
            _world.ActorKilled += OnActorKilled;
        }

        private void OnActorMoved(Actor actor, Vector3 oldPosition, Vector3 newPosition)
        {
            Ellipse circle = _actors[actor];

            Dispatcher.Invoke(() =>
            {
                circle.SetValue(Canvas.LeftProperty, newPosition.X - circle.Width / 2);
                circle.SetValue(Canvas.TopProperty, newPosition.Y - circle.Height / 2);
            });
        }

        private void OnActorKilled(Actor actor, Vector3 vector)
        {
            Debug.WriteLine("Actor died! " + _world.Actors.Count + " left");

            // Remove circle from canvas
            Dispatcher.Invoke(() =>
            {
                if (_children.Contains(_actors[actor]))
                {
                    _children.Remove(_actors[actor]);
                }
            });

            // Remove actor and circle from dictionary
            _actors.Remove(actor);

            // Create death marker
            Dispatcher.Invoke(() =>
            {
                foreach (Shape shape in CreateDeathMarker(vector))
                {
                    _children.Add(shape);
                }
            });

            if (_world.Actors.Count == 0)
            {
                _world.Stop();
                Debug.WriteLine("All actors died, stopped world");
            }
        }

        private void Tick()
        {
            // Do something every tick
        }

        private Actor CreateActor(World world, double x, double y)
        {
            Actor actor = new(world);
            List<Module> modules = new();

            SmartDigestion d = new();
            d.DigestionRate = _random.NextDouble() * 2;
            d.StoredMeat = _random.NextDouble() * 50;
            d.StoredPlant = _random.NextDouble() * 50;
            modules.Add(d);

            SmartEnergy e = new();
            e.EnergyStored = _random.NextDouble() * 10;
            modules.Add(e);

            SmartMovement2D mov = new();
            mov.Speed = _random.NextDouble() * 1 + 1;
            modules.Add(mov);

            BasicBrain brain = new();
            modules.Add(brain);

            actor.AddModule(modules);
            world.Add(actor);

            Ellipse circle = new();
            circle.Width = 10;
            circle.Height = 10;
            byte red = (byte)(255 * _random.NextDouble());
            byte green = (byte)(255 * _random.NextDouble());
            byte blue = (byte)(255 * _random.NextDouble());
            circle.Fill = new SolidColorBrush(Color.FromRgb(red, green, blue));
            circle.Stroke = Brushes.Black;
            circle.StrokeThickness = 1;

            _children.Add(circle);
            _actors.Add(actor, circle);
            actor.Move(new Vector3((float)x, (float)y, 0));

            return actor;
        }

        private List<Shape> CreateDeathMarker(Vector3 position)
        {
            double size = 5;
            List<Shape> shapes = new();

            Line l1 = new();
            l1.X1 = position.X - size;
            l1.X2 = position.X + size;
            l1.Y1 = position.Y - size;
            l1.Y2 = position.Y + size;
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 1;
            shapes.Add(l1);

            Line l2 = new();
            l2.X1 = position.X + size;
            l2.X2 = position.X - size;
            l2.Y1 = position.Y - size;
            l2.Y2 = position.Y + size;
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 1;
            shapes.Add(l2);

            return shapes;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _world.WorldBounds = new Vector3((float)ActorCanvas.ActualWidth, (float)ActorCanvas.ActualHeight, 0);
            _world.WorldWrap = true;
            
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                int r = 100;
                double x = _random.NextDouble() * r - r / 2d + (float)ActorCanvas.ActualWidth / 2;
                double y = _random.NextDouble() * r - r / 2d + (float)ActorCanvas.ActualHeight / 2;

                Actor actor = CreateActor(_world, x, y);
            }

            _world.Start();
        }
    }
}
