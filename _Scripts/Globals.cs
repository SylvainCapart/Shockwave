using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals {

    // Player constants
    public const int EMPTY = 0;
    public const int PLAYER_ONE = 1;
    public const int PLAYER_TWO = 2;
    public const int PLAYER_INCREMENT = 1;
    public const int PLAYER_NUMBER = 2;

    // Game constants
    public const int INITIAL_PIECES_ONE = 12;
    public const int INITIAL_PIECES_TWO = 12;
    public const int POINTS_PER_KILL = 1;
    public const byte FIRE1 = 0;
    public const byte FIRE2 = 1;
    public const float MAX_DISTANCE = 200.0f;

    // Board constants
    public const float BOARD_ORIGIN = 0.0f;
    public const float BOARD_SLOT_WIDTH = 20.0f;
    public const float BOARD_SLOT_HEIGHT= 20.0f;
    public const float BOARD_SLOT_THICKNESS = 0.5f;
    public const byte BOARD_X_SLOT_NB = 9;
    public const byte BOARD_Y_SLOT_NB = 9;

    // Pieces constants
    public const float PIECES_Y_ORIGIN = 1.5f * BOARD_SLOT_THICKNESS + 0.1f;
    public const float PIECES_Y_OUT = 1.0f;
    public const float PIECE_SPEED = 60.0f;
    public const float PIECE_MOVE_TIME = 0.3f;

    // Text positionning constants
    public const float PLAYER_TEXT_X =  62.0f;
    public const float PLAYER_TEXT_Y =  65.0f;
    public const float PLAYER_TEXT_Z = 86.0f;
    public const float PLAYER_TEXT_RX = 90.0f;
    public const float PLAYER_TEXT_RY = 0.0f;
    public const float PLAYER_TEXT_RZ = 0.0f;

    // Timer constants for the coroutines
    public const float GENERIC_TEXT_FADE = 0.5f;
    public const float WIN_TEXT_FADE = 5.0f;
    public const float INFINITY_TEXT_FADE = 360.0f;
    public const float CHECK_ALTITUDE_TIME = 1.0f;
    public const float ENDGAME_TIMER = 2.2f;
    public const float ENDSCREEN_TIMER = 1.5f;
    public const float PLAYER_TEXT_TIME = 1.4f;
    public const float PLAYER_TEXT_APPEAR = 0.3f * PLAYER_TEXT_TIME;
    public const float PLAYER_TEXT_FADE = 0.7f * PLAYER_TEXT_TIME;
    public const float BLINK_TIME = 0.7f;

    // Font polices
    //TODO : examine if the following lines have drawbacks
    public static Font p_fontThirsty = (Font) Resources.Load("_Fonts/ThirstyScriptExtraBoldDemo", typeof(Font));
    public static Font p_fontGames = (Font)Resources.Load("_Fonts/Games", typeof(Font));
    public static Font p_fontGameTime = (Font)Resources.Load("_Fonts/GameTime", typeof(Font));
    public static Font p_fontArial = (Font)Resources.Load("_Fonts/Arial", typeof(Font));
}


