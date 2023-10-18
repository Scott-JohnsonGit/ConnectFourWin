using System;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Class for each piece on visible connect four board
/// </summary>
public class GameVisualPieces : Label
{
	/// <summary>
	/// Column piece is in
	/// </summary>
	public readonly int Column;
	/// <summary>
	/// Row piece is in
	/// </summary>
	public readonly int Row;
    /// <summary>
    /// Set column and row, initalize pieces
    /// </summary>
    /// <param name="column">Column piece is in</param>
    /// <param name="row">Row piece is in</param>
    public GameVisualPieces(int column, int row)
	{
		Column = column;
		Row = row;
	}
	/// <summary>
	/// Change color of visual element
	/// </summary>
	/// <param name="color">New color for piece</param>
	public void ChangeColor(SolidColorBrush color)
	{
		this.Foreground = color;
	}
}
