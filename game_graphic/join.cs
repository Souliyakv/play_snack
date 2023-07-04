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

namespace game_graphic
{
    public partial class join : Form
    {
       public static string newDocumentId;
        public static string name;
        FirestoreDb database;
        public join()
        {
            InitializeComponent();
        }

        private void join_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"snack-game-ad849-firebase-adminsdk-m98k9-84f96a833d.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            database = FirestoreDb.Create("snack-game-ad849");
        }

        private void btn_join_Click(object sender, EventArgs e)
        {
             JoidGame();
        }

        async void JoidGame()
        {
            CollectionReference coll = database.Collection("player");
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                { "name",textBox1.Text},
                { "score",0}
            };
            name = textBox1.Text;
            DocumentReference newDocRef = await coll.AddAsync(data1);
            newDocumentId = newDocRef.Id;
           // Console.WriteLine(coll);
            this.Hide();
            Form1 form1 = new Form1();
            
            form1.Show();
        }
    }
}
