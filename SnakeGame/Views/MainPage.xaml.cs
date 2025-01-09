﻿namespace SnakeGame;

using SharpHook;
using SharpHook.Native;
using SnakeGame.ViewModels;

public partial class MainPage : ContentPage
{
    private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
    {
        { GridValue.Empty, Images.Empty},
        { GridValue.Snake, Images.Body},
        { GridValue.Food, Images.Food},
        { GridValue.Bomb, Images.Bomb},
    };

    private readonly Dictionary<Direction, int> dirToRotate = new()
    {
        { Direction.Up, 0},
        { Direction.Right, 90},
        { Direction.Down, 180},
        { Direction.Left, 270},
    };

    private int rows = 15, cols = 15;
    private Image[,] gridImages;
    private GameState gameState;
    private bool isGameRunning = false;

    private TaskPoolGlobalHook keyboardHook;

    public MainPage()
    {
        InitializeComponent();

        keyboardHook = new TaskPoolGlobalHook();
        keyboardHook.KeyPressed += OnKeyPressed;
        keyboardHook.RunAsync();

        gridImages = SetupGrid();
        gameState = new GameState(rows, cols);

        ScoreText.Text = $"Score: {gameState.Score}";
        HighScoreText.Text = $"High Score: {gameState.HighScore}";
    }

    private Image[,] SetupGrid()
    {
        Image[,] images = new Image[rows, cols];

        RowDefinition[] rowDefinition = new RowDefinition[rows];
        for (int i = 0; i < rows; i++) { rowDefinition[i] = new RowDefinition(GridLength.Star); }
        GameGrid.RowDefinitions = new RowDefinitionCollection(rowDefinition);

        ColumnDefinition[] colDefinition = new ColumnDefinition[rows];
        for (int i = 0; i < rows; i++) { colDefinition[i] = new ColumnDefinition(GridLength.Star); }
        GameGrid.ColumnDefinitions = new ColumnDefinitionCollection(colDefinition);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Image image = new Image { Source = Images.Empty };
                images[i, j] = image;
                GameGrid.Children.Add(image);
                GameGrid.SetRow(image, i);
                GameGrid.SetColumn(image, j);
            }
        }
        return images;
    }

    private async void OnPressedStart(object sender, EventArgs e)
    {
        if (isGameRunning)
            return;

        isGameRunning = true;
        await RunGame();
        isGameRunning = false;
        OverlayText.IsVisible = true;
        gameState = new GameState(rows, cols);
    }

    private async Task RunGame()
    {
        OverlayText.IsVisible = false;
        Draw();
        await GameLoop();
        await DrawDeadSnake();
    }

    private async Task GameLoop()
    {
        while (!gameState.IsGameOver)
        {
            await Task.Delay(100);
            gameState.MoveSnake();
            Draw();
        }
    }

    private void Draw()
    {
        DrawGrid();
        //DrawSnakeHead();
        ScoreText.Text = $"Score: {gameState.Score}";
        HighScoreText.Text = $"High Score: {gameState.HighScore}";
    }

    private void DrawGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GridValue gridValue = gameState.Grid[r, c];
                gridImages[r, c].Source = gridValToImage[gridValue];
                gridImages[r, c].Rotation = 0;
            }
        }
    }

    private void DrawSnakeHead()
    {
        Position headPos = gameState.Snake.Head;
        Image image = gridImages[headPos.Row, headPos.Col];
        image.Source = Images.Head;
        image.Rotation = dirToRotate[gameState.Dir];
    }

    private async Task DrawDeadSnake()
    {
        List<Position> posistions = new List<Position>(gameState.Snake.Body);

        gridImages[posistions[0].Row, posistions[0].Col].Source = Images.DeadHead;
        for (int i = 1; i < posistions.Count; i++)
        {
            Position pos = posistions[i];
            gridImages[pos.Row, pos.Col].Source = Images.DeadBody;
            await Task.Delay(75);
        }
    }

    void OnKeyPressed(object sender, KeyboardHookEventArgs e)
    {
        switch (e.Data.KeyCode)
        {
            case KeyCode.VcUp:
                gameState.ChangeDirection(Direction.Up); break;
            case KeyCode.VcDown:
                gameState.ChangeDirection(Direction.Down); break;
            case KeyCode.VcLeft:
                gameState.ChangeDirection(Direction.Left); break;
            case KeyCode.VcRight:
                gameState.ChangeDirection(Direction.Right); break;
        }
    }

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Up:
                gameState.ChangeDirection(Direction.Up); break;
            case SwipeDirection.Down:
                gameState.ChangeDirection(Direction.Down); break;
            case SwipeDirection.Left:
                gameState.ChangeDirection(Direction.Left); break;
            case SwipeDirection.Right:
                gameState.ChangeDirection(Direction.Right); break;
        }
    }

    private void Up_Pressed(object sender, EventArgs e)
    {
        gameState.ChangeDirection(Direction.Up);
    }
    private void Down_Pressed(object sender, EventArgs e)
    {
        gameState.ChangeDirection(Direction.Down);
    }
    private void Left_Pressed(object sender, EventArgs e)
    {
        gameState.ChangeDirection(Direction.Left);
    }
    private void Right_Pressed(object sender, EventArgs e)
    {
        gameState.ChangeDirection(Direction.Right);
    }
}
