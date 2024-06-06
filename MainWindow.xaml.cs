using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace pract2
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private List<Ellipse> leaves = new List<Ellipse>();
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            DrawTree();
            SetupTimer();
        }

        private void DrawTree()
        {
            // Drawing trunk
            DrawRectangle(390, 300, 20, 100, Brushes.Brown);

            // Drawing branches and leaves
            DrawBranch(400, 300, -90, 100, 10);
        }

        private void DrawRectangle(double x, double y, double width, double height, Brush color)
        {
            Rectangle rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = color
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            drawingCanvas.Children.Add(rect);
        }

        private void DrawBranch(double x1, double y1, double angle, double length, int depth)
        {
            if (depth == 0)
            {
                DrawLeaf(x1, y1);
                return;
            }

            double x2 = x1 + (length * Math.Cos(angle * Math.PI / 180));
            double y2 = y1 + (length * Math.Sin(angle * Math.PI / 180));

            Line branch = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Green,
                StrokeThickness = depth
            };
            drawingCanvas.Children.Add(branch);

            // Recursively draw smaller branches
            DrawBranch(x2, y2, angle - 30, length * 0.7, depth - 1);
            DrawBranch(x2, y2, angle + 30, length * 0.7, depth - 1);
        }

        private void DrawLeaf(double x, double y)
        {
            Ellipse leaf = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Green
            };
            Canvas.SetLeft(leaf, x - 5); // Center the leaf on the endpoint
            Canvas.SetTop(leaf, y - 5);  // Center the leaf on the endpoint
            drawingCanvas.Children.Add(leaf);

            leaves.Add(leaf); // Add leaf to the list for later animation
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Timer interval set to 1 second
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (leaves.Count > 0)
            {
                int leafIndex = random.Next(leaves.Count);
                Ellipse leaf = leaves[leafIndex];
                leaves.RemoveAt(leafIndex);

                AddFallingAnimation(leaf);
            }
        }

        private void AddFallingAnimation(UIElement leaf)
        {
            double duration = random.Next(5, 10); // Random duration between 5 and 10 seconds

            TranslateTransform translateTransform = new TranslateTransform();
            leaf.RenderTransform = translateTransform;

            DoubleAnimation fallAnimation = new DoubleAnimation
            {
                From = 0,
                To = 300, // Fall distance (can be adjusted)
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = false
            };

            fallAnimation.Completed += (s, e) => drawingCanvas.Children.Remove(leaf);

            translateTransform.BeginAnimation(TranslateTransform.YProperty, fallAnimation);
        }
    }
}
