using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using System.Drawing.Imaging;
using System.Media;
using System.IO;

namespace game_graphic
{
    
    public partial class Form1 : Form
    {

        private List<Circle> Snake = new List<Circle>();

        private Circle food = new Circle();


        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;

        FirestoreDb database;
        public string userID;
        string soundfile;
        public Form1()
        {
            InitializeComponent();
            

            new Settings();

        }

        private void play()
        {
          //  byte[] bt = File.ReadAllBytes(soundfile);
            var sound = new SoundPlayer(soundfile);
            sound.PlayLooping();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }



        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        async Task SubscribeToRealtimeUpdates()
        {
            CollectionReference collectionRef = database.Collection("users");
            collectionRef.Listen(snapshot =>
            {

                foreach (DocumentChange change in snapshot.Changes)
                {
                    DocumentSnapshot documentSnapshot = change.Document;
                    if (documentSnapshot.Exists)
                    {
                        // Console.WriteLine("Document ID: " + documentSnapshot.Id);
                      // Console.WriteLine("Document Data: X" + documentSnapshot.ToDictionary()["X"]);
                      //  Console.WriteLine("Document Data: Y" + documentSnapshot.ToDictionary()["Y"]);
                        var foodx = documentSnapshot.ToDictionary()["X"];
                        var foody = documentSnapshot.ToDictionary()["Y"];
                        food = new Circle { X =Convert.ToInt32(foodx) , Y = Convert.ToInt32(foody) };
                    }
                    
                    else
                    {
                        Console.WriteLine("Document deleted: " + documentSnapshot.Id);
                    }
                }
            });

            CollectionReference collectionPlayer = database.Collection("player");
            collectionPlayer.Listen(snap =>
            {

            foreach (DocumentChange change in snap.Changes)
            {
                DocumentSnapshot documentSnap = change.Document;
                if (documentSnap.Exists)
                {
                    // Console.WriteLine("Document ID: " + documentSnapshot.Id);
                  //  Console.WriteLine("Document Data: X" + documentSnap.ToDictionary()["name"]);
                  //  Console.WriteLine("Document Data: Y" + documentSnap.ToDictionary()["score"]);
                    var name = documentSnap.ToDictionary()["name"];
                    var score = documentSnap.ToDictionary()["score"];

                    }

                    else
                    {
                        Console.WriteLine("Document deleted: " + documentSnap.Id);
                    }
                }
            });

        }

        private async void StartGame(object sender, EventArgs e)
        {
            SubscribeToRealtimeUpdates();
            RestartGame();
            soundfile = "sound.wav";
            play();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "I scored: " + score + " and my Highscore is " + highScore + " on the Snake Game from MOO ICT";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.Purple;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game SnapShot MOO ICT";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }





        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // setting the directions

            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            // end of directions

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {

                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }


                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {

                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }

                    }


                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }


            picCanvas.Invalidate();


        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }


            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
            food.X * Settings.Width,
            food.Y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = join.name;
            userID = join.newDocumentId;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"snack-game-ad849-firebase-adminsdk-m98k9-84f96a833d.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            database = FirestoreDb.Create("snack-game-ad849");
          //  MessageBox.Show(database.ToString());
        }

        private void picCanvas_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            RestartGame();

        }

        private void snapButton_Click(object sender, EventArgs e)
        {
          
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void RestartGame()
        {

            //MessageBox.Show(WiHe.ToString());
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();

            startButton.Enabled = false;

            score = 0;
            txtScore.Text = "Score: " + score;

            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head); // adding the head part of the snake to the list

            for (int i = 0; i < 3; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

           // food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            gameTimer.Start();

        }

        private void EatFood()
        {
            score += 1;

            txtScore.Text = "Score: " + score;
            string soundfilePath = "eat.wav";
            var sound = new SoundPlayer(soundfilePath);
            sound.Play();

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

           Snake.Add(body);
            DocumentReference docRefScore = database.Collection("player").Document(userID);

            var updatesScore = new Dictionary<FieldPath, object>
    {
       // { new FieldPath("X"), rand.Next(2, maxWidth) },
        { new FieldPath("score"), score},
        // Add more field updates as needed
    };

            docRefScore.UpdateAsync(updatesScore);

            // food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            DocumentReference docRef = database.Collection("users").Document("b6gGjObddDW0GTuU3s9a");

            var updates = new Dictionary<FieldPath, object>
    {
        { new FieldPath("X"), rand.Next(2, maxWidth) },
        { new FieldPath("Y"), rand.Next(2, maxHeight)},
        // Add more field updates as needed
    };

            docRef.UpdateAsync(updates);
            soundfile = "sound.wav";
            play();

        }

        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            string soundfilePath = "gameover.wav";
            var sound = new SoundPlayer(soundfilePath);
            sound.Play();

            if (score > highScore)
            {
                highScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }


    }
}
