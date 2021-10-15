using UnityEngine;


public class DatabasePuzzle02_02 : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;
    
    public transformMatrix[] transforms0;
    public transformMatrix[] transforms1;
    public transformMatrix[] transforms2;
    public transformMatrix[] transforms3;
    public transformMatrix[] transforms4;
    public transformMatrix[] transforms5;

    transformMatrix t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24;

    public int[,] tragetMatracies = new int[,] { {20, 22}, {30, 50}, {10 ,40}, {4, 60}, {-20, 55}, {-50, 40} }; 

    public void InitDatabase()
    {
        Debug.Log("Connect GameController of Puzzle02");
        GCP02 = GetComponent<GameControllerPuzzle02>();

        refreshDatabase();
    }
  
    public void refreshDatabase()
    {
        //question 1 (20, 22)
        t1.values = new int[]  { 20, 0,  // correct uses (1,1)
                                 0, 22 };    
        t2.values = new int[]  { 20, 0,  // correct uses (1,0)
                                 22, 0 };
        t3.values = new int[]  { 0, 0, 
                                 20, 22 };
        t4.values = new int[]  { 10, 11,  
                                 10, 11 };
        //question 2 (30, 50)
        t5.values = new int[] { 0, 0, 
                                30, 50 };
        t6.values = new int[] { 15, 25, 
                                15, 25 };
        t7.values = new int[] { 15, 15, 
                                25, 25 };   //correct uses (1,1)
        t8.values = new int[] { 30, 15, 
                                25, 50 };
        //question 3 (10, 40)
        t9.values = new int[] { -10, -30, 
                                40, 20 };     
        t10.values = new int[] { 50, 30, 
                                 60, 30 };
        t11.values = new int[] { 10, 0, 
                                 -10, -20 };
        t12.values = new int[] { 30, 20,   //correct uses (1,-1)
                                 70, 30 };
        //question 4 (4, 60)
        t13.values = new int[] { 20, 16, 
                                 44, -16 }; //correct uses (1,-1)
        t14.values = new int[] { 0, 4, 
                                 60, 10 };     
        t15.values = new int[] { 30, 26, 
                                 40, 10 };
        t16.values = new int[] { 50, 10, 
                                 2, 2 };
        //question 5 (-20, 55)
        t17.values = new int[] { 30, -20, 
                                 40, 15 };     
        t18.values = new int[] { 30, -20,  // correct uses (0,1)
                                 40, 55 };
        t19.values = new int[] { -10, -10, 
                                 -45, -10 };
        t20.values = new int[] { 15, -5, 
                                 30, 25 };
        //question 6 (-50, 40)
        t21.values = new int[] { -35, -25, 
                                 -30, 10 };      
        t22.values = new int[] { 30, 20, 
                                 30, -10 };
        t23.values = new int[] { -35, 15,  // correct uses (1,-1)
                                 65, 25 };
        t24.values = new int[] { -10, -40, 
                                 20, 20 };      

        transforms0 = new transformMatrix[] { t1, t2, t3, t4 };
        transforms1 = new transformMatrix[] { t5, t6, t7, t8 };
        transforms2 = new transformMatrix[] { t9, t10, t11, t12 };
        transforms3 = new transformMatrix[] { t13, t14, t15, t16 };
        transforms4 = new transformMatrix[] { t17, t18, t19, t20 };
        transforms5 = new transformMatrix[] { t21, t22, t23, t24 };
    }

    // multiply given matrix by given vector and return result
    public int[] calculation(int[] vector, int[] matrix)
    {
        int[] result = { 0, 0 };

        result[0] = matrix[0] * vector[0] + matrix[1] * vector[1];
        result[1] = matrix[2] * vector[0] + matrix[3] * vector[1];

        refreshDatabase();

        return result;
    }
}
