using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using Descrambler.Properties;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Descrambler
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// 2D array used to maintain letters entered into GUI.
    /// </summary>
    private string[,] letters = new string[4, 4];
    /// <summary>
    /// Data structure to hold the text boxes for easy access and looping.
    /// </summary>
    private TextBox[,] txtBoxes = new TextBox[4, 4];
    /// <summary>
    /// Dictionary of valid words gets parsed into here.
    /// </summary>
    private string[] words;
    /// <summary>
    /// Random # generator.  Used for choosing random letters.
    /// </summary>
    private Random rand = new Random();
    /// <summary>
    /// Found answers are stored here.
    /// </summary>
    List<string> answers = new List<string>();
    /// <summary>
    /// This contains valid words in a hash table
    /// </summary>
    Dictionary<string, string> validWords = new Dictionary<string, string>();
    /// <summary>
    /// This contrains valid word starts in a hash table.  This can be used to prune the recursive tree.
    /// </summary>
    Dictionary<string, string> wordStarts = new Dictionary<string, string>();

    public Form1()
    {
      InitializeComponent();

      txtBoxes[0, 0] = txtBox00;
      txtBoxes[0, 1] = txtBox01;
      txtBoxes[0, 2] = txtBox02;
      txtBoxes[0, 3] = txtBox03;

      txtBoxes[1, 0] = txtBox10;
      txtBoxes[1, 1] = txtBox11;
      txtBoxes[1, 2] = txtBox12;
      txtBoxes[1, 3] = txtBox13;

      txtBoxes[2, 0] = txtBox20;
      txtBoxes[2, 1] = txtBox21;
      txtBoxes[2, 2] = txtBox22;
      txtBoxes[2, 3] = txtBox23;

      txtBoxes[3, 0] = txtBox30;
      txtBoxes[3, 1] = txtBox31;
      txtBoxes[3, 2] = txtBox32;
      txtBoxes[3, 3] = txtBox33;

      // Parse out dictionary of valid words
      words = Resources.enable1.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      foreach (string word in words)
      {
        validWords.Add(word, word);

        for (int i = 0; i < word.Length - 2; i++)
        {
          string sub = word.Substring(0, i + 1);
          if (!wordStarts.ContainsKey(sub))
          {
            wordStarts.Add(sub, sub);
          }
        }
      }
    }

    private void btnDescramble_Click(object sender, EventArgs e)
    {
      EnableControls(false);

      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          letters[i, j] = txtBoxes[i, j].Text;
        }
      }

      // Clear out any old results
      answers.Clear();
      listBox1.Items.Clear();

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      // Recursively build words starting from each of the 16 locations on the grid.
      Parallel.For(0, 4, i =>
      {
        for (int j = 0; j < 4; j++)
        {
          buildWord(new List<Tile>(), i, j);
        }
      });

      stopwatch.Stop();
      Console.WriteLine("Time elapsed: " + stopwatch.Elapsed);
      listBox1.Items.AddRange(answers.ToArray());

      EnableControls(true);
    }

    /// <summary>
    /// Recusrive function that takes an input, adds the current location, and checks its validity.
    /// Then calls itself in all the other next possible play directions.
    /// </summary>
    /// <param name="tiles">Tiles representing the word built so far</param>
    /// <param name="row">The current row index</param>
    /// <param name="col">The current col index</param>
    private void buildWord(List<Tile> tiles, int row, int col)
    {
      // Not a valid move
      if (row > 3 || col > 3 || row < 0 || col < 0)
        return;

      foreach (Tile tile in tiles)
      {
        // Check to see if we already contain this tile in our solution, if so we've intersected our solution which
        // isn't a valid move
        if (row == tile.row && col == tile.col)
          return;
      }

      // Build a tile for the current location
      Tile newTile = new Tile();
      newTile.letter = letters[row, col];
      newTile.row = row;
      newTile.col = col;
      tiles.Add(newTile);

      // Build up the current word
      string word = "";
      foreach (Tile tile in tiles)
        word += tile.letter;

      System.Console.WriteLine(word + " " + row + " " + col);
      if (validWords.ContainsKey(word))
        answers.Add(word);
    

      // If no possible words even begin with this current combination of letters, we can prune this 
      // branch of our recursive tree because we will not find any more valid results down this branch.
      if (!wordStarts.ContainsKey(word))
        return;

      // Call with possible location one row above current location
      buildWord(new List<Tile>(tiles), row - 1, col - 1);
      buildWord(new List<Tile>(tiles), row - 1, col);
      buildWord(new List<Tile>(tiles), row - 1, col + 1);

      // Call with possible location with row same as current location
      buildWord(new List<Tile>(tiles), row, col - 1);
      buildWord(new List<Tile>(tiles), row, col + 1);

      // Call with possible location one row below current location
      buildWord(new List<Tile>(tiles), row + 1, col - 1);
      buildWord(new List<Tile>(tiles), row + 1, col);
      buildWord(new List<Tile>(tiles), row + 1, col + 1);
    }

    private void btnRandomize_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          txtBoxes[i,j].Text = ((char)('a' + rand.Next(0, 26))).ToString();
        }
      }
    }

    /// <summary>
    /// Convenience function to enable/disable controls that need to be changed in a batch.
    /// </summary>
    /// <param name="flag"></param>
    public void EnableControls(bool flag)
    {
      btnDescramble.Enabled = flag;
      btnRandomize.Enabled = flag;
      foreach (TextBox txtBox in txtBoxes)
        txtBox.Enabled = flag;
      Application.DoEvents();
    }
  }
}
