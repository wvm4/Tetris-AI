using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Threading.Tasks;
using Unity.MLAgents.Sensors.Reflection;
using Unity.VisualScripting;

public class TetrisAI : Agent
{
    [SerializeField] Player player;
    [SerializeField] Piece piece;
    public Main main;
    public TetrisAI opponentAI;
    public bool firstLoop;

    public override void OnActionReceived(ActionBuffers actions)
    {
        piece.nextMovement = AIOutputs(actions.DiscreteActions[0]);
        piece.nextRotation = AIOutputs(actions.DiscreteActions[1]);
        piece.nextDrop = AIOutputs(actions.DiscreteActions[2]);
        piece.nextHold = actions.DiscreteActions[3];

        //piece.nextMovement = actions.DiscreteActions[0];
        //piece.nextRotation = actions.DiscreteActions[1];
        //piece.nextDrop = actions.DiscreteActions[2];
        //piece.nextHold = actions.DiscreteActions[3];
    }

    public int AIOutputs(int action)
    {
        int nextInput = 0;
        switch (action)
        {
            default:
                nextInput = 0;
                break;
            case 1:
                nextInput = 1;
                break;
            case 2:
                nextInput = -1;
                break;
        }
        return nextInput;
    }







    public override void CollectObservations(VectorSensor sensor)
    {
        for (int i = 0; i < player.field.GetLength(0); i++)
        {
            for (int j = 0; j < player.field.GetLength(1); j++)
            {
                sensor.AddObservation(player.field[i, j]);
            }
        }



        sensor.AddObservation(ObserveTetromino(player.holdingPiece));

        for (int i = 0; i < 5; i++)
        {
            sensor.AddObservation(ObserveTetromino(player.tetrominoBag1[i]));
        }
        sensor.AddObservation(player.opponent.highestPieceHeight);
        sensor.AddObservation(piece.position - player.fieldOffset);
        sensor.AddObservation(piece.rotationIndex);

        for (int i = 0; i < 4; i++)
        {
            sensor.AddObservation(piece.blocks[i]);
        }

    }

    public float NormalizeObservationValue(int value, int maximum, int minimum)
    {
        if (value > 0)
        {
            return (((float)value / ((float)maximum )));
        } else if (value < 0)
        {
            return (((float)value) / ((float)maximum));
        } else
        {
            return 0;
        }
    }

   

    public int ObserveTetromino(TetrominoData tetrominoData)
    {
        switch (tetrominoData.tetromino)
        {
            default:
                return 0;
            case Tetromino.I:
                return 1;
            case Tetromino.O:
                return 2;
            case Tetromino.T:
                return 3;
            case Tetromino.J:
                return 4;
            case Tetromino.S:
                return 5;
            case Tetromino.L:
                return 6;
            case Tetromino.Z:
                return 7;
        }
    }

    public override void OnEpisodeBegin()
    {
        if (!main.episodeAlreadyStarted)
        {
            main.episodeAlreadyStarted = true;
            main.ResetPlayers();
        }
    }
}
