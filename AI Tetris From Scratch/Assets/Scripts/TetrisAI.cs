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
    public StatsRecorder statsRecorder;
    

    public override void OnActionReceived(ActionBuffers actions)
    {
        //piece.nextMovement = AIOutputs(actions.DiscreteActions[0]);
        //piece.nextRotation = AIOutputs(actions.DiscreteActions[1]);
        //piece.nextDrop = AIOutputs(actions.DiscreteActions[2]);
        //piece.nextHold = actions.DiscreteActions[3];

        //switch (actions.DiscreteActions[0])
        //{
        //    case 0:
        //        piece.nextDrop = -1;
        //        break;
        //    case 1:
        //        piece.nextDrop = 1;
        //        break;
        //    case 2:
        //        piece.nextMovement = 1;
        //        break;
        //    case 3:
        //        piece.nextMovement = -1;
        //        break;
        //    case 4:
        //        piece.nextRotation = 1;
        //        break;
        //    case 5:
        //        piece.nextRotation = -1;
        //        break;
        //    case 6:
        //        piece.nextHold = 1;
        //        break;
        //}

        piece.tryPosition.x = actions.DiscreteActions[0] - 5;


        piece.tryRotationIndex = actions.DiscreteActions[1];

        piece.nextHold = actions.DiscreteActions[2];

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

        //for (int i = 0; i < player.fieldSize.x; i++)
        //{
        //    sensor.AddObservation(NormalizeObservationValue(player.boardHeightMap[i], player.fieldSize.y, 0));
        //}

        ObserveTetromino(player.holdingPiece, sensor);

        for (int i = 0; i < 3; i++)
        {
            ObserveTetromino(player.tetrominoBag1[i], sensor);
        }

        sensor.AddObservation(piece.previousHold);
        //sensor.AddObservation(NormalizeObservationValue(player.opponent.highestPieceHeight, 28, 0));
        //sensor.AddObservation(NormalizeObservationValue(piece.position.x - player.fieldOffset.x, 10, 0));
        //sensor.AddObservation(NormalizeObservationValue(piece.position.y - player.fieldOffset.y, 28, 0));
        //sensor.AddObservation(NormalizeObservationValue(piece.rotationIndex, 3, 0));

        //for (int i = 0; i < 4; i++)
        //{
        //    sensor.AddObservation(NormalizeObservationValue(piece.blocks[i].x, 10, 0));
        //    sensor.AddObservation(NormalizeObservationValue(piece.blocks[i].y, 28, 0));
        //}

        //sensor.AddObservation(NormalizeObservationValue(player.linesClearedTotal, 1000, 0));

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

   

    public void ObserveTetromino(TetrominoData tetrominoData, VectorSensor sensor)
    {
        switch (tetrominoData.tetromino)
        {
            default:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.I:
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.O:
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.T:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.J:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.S:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                break;
            case Tetromino.L:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                sensor.AddObservation(0);
                break;
            case Tetromino.Z:
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(1);
                break;
        }
    }

    public override void OnEpisodeBegin()
    {
        statsRecorder.Add("Environment/Lines Cleared", player.linesClearedTotal, StatAggregationMethod.Histogram);

        if (!main.episodeAlreadyStarted)
        {
            main.episodeAlreadyStarted = true;
            main.ResetPlayers();
        }
    }
}
