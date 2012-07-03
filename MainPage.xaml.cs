/*
 Developed by Igor AMorim Costa Portela
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Projeto_WP7___Igor_Amorim_C.Portela
{
    public partial class MainPage : PhoneApplicationPage
    {
        int nivel = 1;
        int count = 0;
        int points = 0;
        int lifes = 3;
        int seconds = 500;
        int bgcolor = 10;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer circleChange = new DispatcherTimer();
        List<Shape> circles = new List<Shape>();
        Random r = new Random();
        List<int> pointList = new List<int>();


        public MainPage()
        {
            InitializeComponent();

            personalTile();
            circleChange.Interval = TimeSpan.FromMilliseconds(100);
            circleChange.Tick += new EventHandler(circleChange_Tick);
            circleChange.Start();
            timer.Interval = TimeSpan.FromMilliseconds(seconds);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

            timer.Start();
            pivotGame.SelectedIndex = 1;
        }

        // Showing bubbles
        void timer_Tick(object sender, EventArgs e)
        {
            createBubble();

            count++;

            switch (count)
            {
                case 10:
                    nivel = 2;
                    break;
                case 20:
                    nivel = 3;
                    break;
            }

            Nivel.Text = "Nível " + nivel.ToString();
            Pointing.Text = "Pontuação " + points.ToString();
            Lifes.Text = "Vidas " + lifes.ToString();

        }

        // Creating bubble
        void createBubble()
        {
            var x = r.Next(360) + 20;
            var y = r.Next(450);


            Ellipse elipse = new Ellipse();
            elipse.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(bgcolor * 5), (byte)(bgcolor * 3), (byte)(bgcolor * 1)));
            elipse.Margin = new Thickness(x, y, 0, 0);
            elipse.Width = 50;
            elipse.Height = 50;
            bgcolor += 10;

            circles.Add(elipse);
            contentPanel.Children.Add(elipse);

            elipse.MouseLeftButtonDown += new MouseButtonEventHandler(elipse_MouseLeftButtonDown);

        }

        // Click bubble
        void elipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Shape)sender).Visibility = System.Windows.Visibility.Collapsed;
            circles.Remove((Shape)sender);

            switch (nivel)
            {
                case 1:
                    points += 10;
                    break;
                case 2:
                    points += 20;
                    break;
                case 3:
                    points += 30;
                    break;
            }

        }

        // Ranking button
        private void btnRanking_Click(object sender, RoutedEventArgs e)
        {
            pivotGame.SelectedIndex = 2;
            lstRanking.ItemsSource = readRanking();
        }

        // Growwing circle
        void circleChange_Tick(object sender, EventArgs e)
        {
            foreach (var i in circles)
            {
                switch (nivel)
                {
                    case 1:
                        i.Width += 10;
                        i.Height += 10;
                        break;
                    case 2:
                        i.Width += 20;
                        i.Height += 20;
                        break;
                    case 3:
                        i.Width += 30;
                        i.Height += 30;
                        break;
                }

                if (i.Width >= 400)
                {
                    if (lifes > 0)
                    {
                        lifes--;
                        saveRanking();
                        count = 0;
                        points = 0;
                        nivel = 1;
                    }
                    else
                    {
                        circleChange.Stop();
                        pivotGame.SelectedIndex = 0;
                    }

                    i.Visibility = System.Windows.Visibility.Collapsed;
                    circles.Remove(i);
                    MessageBox.Show("Você perdeu!");
                    break;
                }
            }
        }

        // Inserting in ranking
        private void saveRanking()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            List<int> list = new List<int>();
            if (settings.Contains("pointing"))
            {
                list = (List<int>)settings["pointing"];
                list.Add(points);
                settings["pointing"] = list;
            }
            else
            {
                pointList.Add(points);
                settings["pointing"] = pointList;
            }

            settings.Save();
        }

        // Showing ranking
        private List<int> readRanking()
        {

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            //List<int> listaPontos = new List<int>();
            List<int> resultList = new List<int>();
            

            if (settings.Contains("pointing"))
            {
                pointList = (List<int>)settings["pointing"];
                var result = pointList.OrderByDescending(n => n).Take(3);

                foreach (var i in result)
                {
                    resultList.Add(i);
                }
            }
            else {
                MessageBox.Show("Nenhum dado cadastrado!");
                pivotGame.SelectedIndex = 0;
            }

            return resultList;
        }

        //Tile
        public void personalTile()
        {
            var tile = ShellTile.ActiveTiles.First();
            if (tile != null)
            {
                var standardTile = new StandardTileData
                {
                    Title = "Bubbles",
                    BackTitle = "in your finger",
                    BackgroundImage = new Uri("images/bubble.png", UriKind.Relative),
                    BackBackgroundImage = new Uri("images/finger.png", UriKind.Relative)
                };

                tile.Update(standardTile);
            }
        }
    }
}