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

namespace Descrambler
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// 2D array used to maintain letters entered into GUI.
    /// </summary>
    private string[,] letters = new string[4, 4];
    private string[] words;   

    public Form1()
    {
      InitializeComponent();
      // Parse out dictionary of valid words
      words = Resources.enable1.Split(new string[] { "\r\n" }, StringSplitOptions.None);    
    }

    private void Button1_Click(object sender, EventArgs e)
    {
      Button1.Enabled = false;
      Application.DoEvents();

      // Read input into 2D array
      letters[0,0] = txtBox00.Text;
      letters[0,1] = txtBox01.Text;
      letters[0,2] = txtBox02.Text;
      letters[0,3] = txtBox03.Text;
      
      letters[1,0] = txtBox10.Text;
      letters[1,1] = txtBox11.Text;
      letters[1,2] = txtBox12.Text;
      letters[1,3] = txtBox13.Text;
      
      letters[2,0] = txtBox20.Text;
      letters[2,1] = txtBox21.Text;
      letters[2,2] = txtBox22.Text;
      letters[2,3] = txtBox23.Text;
      
      letters[3,0] = txtBox30.Text;
      letters[3,1] = txtBox31.Text;
      letters[3,2] = txtBox32.Text;
      letters[3,3] = txtBox33.Text;

      // Clear out any old results
      listBox1.Items.Clear();

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      // Recursively build words starting from each of the 16 locations on the grid.
      buildWord(new List<Tile>(), 0, 0);
      buildWord(new List<Tile>(), 0, 1);
      buildWord(new List<Tile>(), 0, 2);
      buildWord(new List<Tile>(), 0, 3);

      buildWord(new List<Tile>(), 1, 0);
      buildWord(new List<Tile>(), 1, 1);
      buildWord(new List<Tile>(), 1, 2);
      buildWord(new List<Tile>(), 1, 3);

      buildWord(new List<Tile>(), 2, 0);
      buildWord(new List<Tile>(), 2, 1);
      buildWord(new List<Tile>(), 2, 2);
      buildWord(new List<Tile>(), 2, 3);

      buildWord(new List<Tile>(), 3, 0);
      buildWord(new List<Tile>(), 3, 1);
      buildWord(new List<Tile>(), 3, 2);
      buildWord(new List<Tile>(), 3, 3);

      stopwatch.Stop();
      Console.WriteLine("Time elapsed: " + stopwatch.Elapsed);

      Button1.Enabled = true;
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

      bool startsWithWord = false;
      foreach (string dictionaryWord in words)
      {
        if (word == dictionaryWord)
        {
          // This is a valid word, so add it to our results
          listBox1.Items.Add(word);
          Application.DoEvents();
        }

       if (dictionaryWord.StartsWith(word))
          startsWithWord = true;
      }

      // If no possible words even begin with this current combination of letters, we can prune this 
      // branch of our recursive tree because we will not find any more valid results down this branch.
      if (!startsWithWord)
        return;

      List<int> rowList = new List<int>();
      rowList.Add(row - 1);
      rowList.Add(row);
      rowList.Add(row + 1);
      List<int> colList = new List<int>();
      colList.Add(col - 1);
      colList.Add(col);
      colList.Add(col + 1);

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
  }
}
