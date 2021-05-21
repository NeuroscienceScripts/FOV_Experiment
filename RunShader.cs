using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace;
using UnityEditor.VersionControl;
using UnityEngine;

public class RunShader : MonoBehaviour
{
    public monocularSettings monocularSettings; 
    public Material setLinesMaterial;
    public Material displayLightsMaterial;
    public Material blindSpotMaterial; 
    public Camera mainCam; 

    public GameObject lineInstructions;
    public GameObject perimeterInstructions;

    private bool runAll; 
    private bool linesSet;
    private bool blindspotSet; 
    private bool hasStarted;
    private float timeTracker;

    private float xRange = .25f;
    private float yRange = .5f;
    
    private const int numberRepetitions = 1;
    private const int numberOfBlankTrialsPerRepetition = 4; 
    private const int numberOfStepsEachDirection = 3;
    private const int numberLayersEachDirection = 5;
    private const float layerSize = .005f; 
    private Vector4[] xyList;

    private int blindspotX;
    private int blindspotY;

    private int[] trialResponseArray;
    private int trialNumber;
    private int pause=0;
    private string subjectFile; 

    private void Start()
    {
        
        displayLightsMaterial.SetInt("debugMode", 0);
        
        Debug.Log(mainCam.fieldOfView);
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar +"Data" + Path.DirectorySeparatorChar
                      + "Experiment_Data" + DateTime.Now.Date.ToShortDateString() + DateTime.Now.Date.ToShortTimeString();

        if (monocularSettings == monocularSettings.All)
        {
            runAll = true;
            monocularSettings = monocularSettings.Left;
            yRange = .5f; 
        }
        else if (monocularSettings == monocularSettings.Both)
            yRange = .25f;
        else if (monocularSettings == monocularSettings.Right)
            xRange = .75f; 
        
    }
    
    /// <summary>
    /// x/y location for a light to appear (in screen coordinates),
    /// if it's testing x or y fov
    /// which step away from the center line it resides in.
    /// </summary>
    private void setList()
    {
        xyList = new Vector4[20 * numberRepetitions * numberOfStepsEachDirection * numberLayersEachDirection + numberRepetitions * numberOfBlankTrialsPerRepetition];
        int count = 0;
        foreach(int repetition in Enumerable.Range(0, numberRepetitions))
        {
            foreach(int xyOffset in Enumerable.Range(0, numberOfStepsEachDirection))
            {
                foreach (int layer in Enumerable.Range(0, numberLayersEachDirection))
                {
                    xyList[count] = new Vector4(.5f + xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f + yRange + layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f + yRange + layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f + xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f + yRange - layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f + yRange - layer *layerSize, 0f, (float)layer);
                    count++;
                    
                    xyList[count] = new Vector4(.5f + xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f - yRange + layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f - yRange + layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f + xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f - yRange - layer *layerSize, 0f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xyOffset * (.5f/(numberOfStepsEachDirection+1)), .5f - yRange - layer *layerSize, 0f, (float)layer);
                    count++;

                    xyList[count] = new Vector4(.5f + xRange + layer * layerSize, .5f + xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f + xRange + layer * layerSize, .5f - xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f + xRange - layer * layerSize, .5f + xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f + xRange - layer * layerSize, .5f - xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    
                    xyList[count] = new Vector4(.5f - xRange + layer * layerSize, .5f + xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xRange + layer * layerSize, .5f - xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xRange - layer * layerSize, .5f + xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                    xyList[count] = new Vector4(.5f - xRange - layer * layerSize, .5f - xyOffset *(.5f/(numberOfStepsEachDirection+1)), 1f, (float)layer);
                    count++;
                }
            }
            foreach(int x in Enumerable.Range(0, numberOfBlankTrialsPerRepetition))
            {
                xyList[count] = new Vector4(1.1f, 1.1f, 2f, 0);
                count++; 
            }
            foreach(int x in Enumerable.Range(0, numberOfBlankTrialsPerRepetition))
            {
                xyList[count] = new Vector4(blindspotX, blindspotY, 2f, 0);
                count++; 
            }

            //TODO randomize order
            trialResponseArray = new int[xyList.Length];
        }
    }

    // Update is called once per frame
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
       if (monocularSettings == monocularSettings.Left)
       {
            if(Camera.main)
                Camera.main.stereoTargetEye = StereoTargetEyeMask.Left; 
       }   
       else if (monocularSettings == monocularSettings.Right)
       {
           if(Camera.main)
            Camera.main.stereoTargetEye = StereoTargetEyeMask.Right; 
       }   
        
       if ( !blindspotSet && (monocularSettings == monocularSettings.Left || monocularSettings == monocularSettings.Right))
       {
           Debug.Log("Setting blindspot"); 
           blindSpotMaterial.SetFloat("aspectRatio", (float)source.width/(float)source.height);
            Debug.Log(xRange + "," + yRange);
            if (Input.GetKey("up"))
                yRange += .001f;

            if (Input.GetKey("down"))
                yRange += -.001f;

            if (Input.GetKey("right"))
                xRange += .001f;

            if (Input.GetKey("left"))
                xRange += -.001f;

            if (Input.GetKey("space"))
            {
                blindspotSet = true;
                lineInstructions.SetActive(true); 
                //perimeterInstructions.SetActive(true);

                xRange = .1f;
                yRange = .1f; 
            }
            else
            {
                blindSpotMaterial.SetFloat("xValue", xRange);
                blindSpotMaterial.SetFloat("yValue", yRange);

                Graphics.Blit(source, destination, blindSpotMaterial);
            }
            
       }
       else if (!linesSet)
       {
           if (timeTracker > .5)
           {
               Debug.Log("Setting FOV start");
               displayLightsMaterial.SetFloat("aspectRatio", (float) source.width / (float) source.height);

               if (Input.GetKey("up"))
                   yRange += .001f;

               if (Input.GetKey("down"))
                   yRange += -.001f;

               if (Input.GetKey("right"))
                   xRange += .001f;

               if (Input.GetKey("left"))
                   xRange += -.001f;

               if (Input.GetKey("space"))
               {
                   linesSet = true;
                  // setList();
                   perimeterInstructions.SetActive(true);
                   timeTracker = 0.0f; 
               }
               else
               {
                   setLinesMaterial.SetFloat("xValue", xRange);
                   setLinesMaterial.SetFloat("yValue", yRange);

                   Graphics.Blit(source, destination, setLinesMaterial);
               }
           }
           else timeTracker += Time.deltaTime; 
       }
       else
       {
            if (!hasStarted)
            {
                Debug.Log("Waiting for spacebar to start"); 
                if (Input.GetKey("space") && timeTracker > .5)
                {
                    hasStarted = true;
                    perimeterInstructions.SetActive(false);
                }
                else
                {
                    timeTracker += Time.deltaTime; 
                }

                Graphics.Blit(source, destination); 
            }
            else
            {
                Debug.Log("loc: " + trialNumber);
                Debug.Log("xVal: " + xyList[trialNumber].x);
                Debug.Log("yVal: " + xyList[trialNumber].y);
                displayLightsMaterial.SetFloat("xValue", xyList[trialNumber].x);
                displayLightsMaterial.SetFloat("yValue", xyList[trialNumber].y);

                Graphics.Blit(source, destination, displayLightsMaterial);

                if (Input.GetKey("space") && timeTracker > .5)
                    trialResponseArray[trialNumber] = 1; 

                if (pause % 100 == 0)
                    trialNumber++;
                pause++;
            }
       }
        
       // if(runAll)
    }
}
