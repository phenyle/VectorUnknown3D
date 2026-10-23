using System;
using System.Globalization;
using SimpleJSON;
using UnityEngine;

/// <summary>
/// One play of a 2D level or a 3D puzzle, for the teachers' "attempts" tab. The game adds the moves
/// the player submits (GO, Fire, Submit) and, at the end, the outcome; the server then stamps the
/// row with the player, class section and time, works out the concept, totals and mistake counts,
/// and rebuilds the "Students" and "Overview" tabs from it (Server/AppsScript/Code.gs, logAttempt).
///
/// Finish sends the row once. A restart or an abandoned play with no moves is not sent: there is
/// nothing to learn from it.
/// </summary>
public class AttemptLog
{
    public const string Completed = "completed", Failed = "failed", Restarted = "restarted", Abandoned = "abandoned";

    private const int MaxMoves = 300;

    private readonly JSONObject data = new JSONObject();
    private readonly JSONArray moves = new JSONArray();
    private readonly Func<float> clock;   // active seconds so far (paused time excluded by the caller)
    private bool finished;

    /// <param name="game">"2D" or "3D"</param>
    /// <param name="activity">"L1".."L7" (2D levels) or "P1".."P4" (3D puzzles)</param>
    /// <param name="stage">3D puzzle stage (1-3), 0 when the activity has none</param>
    /// <param name="difficulty">2D difficulty (1 easy - 3 hard), 0 when not used</param>
    /// <param name="problem">the task in readable form, e.g. "Reach ⟨-10, 5⟩ with cards ..."</param>
    /// <param name="idealMoves">fewest submitted moves that finish the play</param>
    public AttemptLog(string game, string activity, int stage, int difficulty, string problem, int idealMoves, Func<float> clock)
    {
        this.clock = clock;
        data["game"] = game;
        data["activity"] = activity;
        data["stage"] = stage;
        data["difficulty"] = difficulty;
        data["problem"] = problem;
        data["ideal_moves"] = idealMoves;
    }

    public bool IsFinished { get { return finished; } }
    public int MoveCount { get { return moves.Count; } }

    /// <param name="move">readable move, e.g. "2·⟨1, 0⟩ + 3·⟨0, 1⟩ = ⟨2, 3⟩"</param>
    /// <param name="mistake">a MathMistakes category, or "" when the move can't be judged on its own</param>
    public void AddMove(string move, string mistake)
    {
        if (finished || moves.Count >= MaxMoves)
            return;
        JSONObject m = new JSONObject();
        m["t"] = Seconds();
        m["move"] = move;
        m["mistake"] = mistake;
        moves.Add(m);
    }

    /// <param name="score">the game's own score (2D stars 1-3, 3D star score), NaN if none</param>
    /// <param name="efficiency">ideal / actual effort when the game measures it better than
    /// ideal moves / submitted moves (Puzzle 4 counts every edit); NaN to let the server work it out</param>
    public void Finish(string outcome, float score = float.NaN, string finalAnswer = "", float efficiency = float.NaN)
    {
        if (finished)
            return;
        finished = true;
        if ((outcome == Restarted || outcome == Abandoned) && moves.Count == 0)
            return;

        data["outcome"] = outcome;
        data["seconds"] = Seconds();
        if (!float.IsNaN(score))
            data["score"] = Mathf.Round(score * 100f) / 100f;
        if (!float.IsNaN(efficiency))
            data["efficiency"] = Mathf.Round(efficiency * 100f) / 100f;
        data["final_answer"] = finalAnswer;
        data["moves"] = moves;
        GameRootMaster.GSFU.LogAttempt(data);
    }

    private float Seconds()
    {
        return Mathf.Round(clock() * 10f) / 10f;
    }

    // ---- readable math ----------------------------------------------------------------------

    public static string Num(float f)
    {
        return f.ToString("0.##", CultureInfo.InvariantCulture);
    }

    public static string Vec(Vector2 v)
    {
        return "⟨" + Num(v.x) + ", " + Num(v.y) + "⟩";
    }

    public static string Vec(Vector3 v)
    {
        return "⟨" + Num(v.x) + ", " + Num(v.y) + ", " + Num(v.z) + "⟩";
    }

    /// <summary>"2·⟨1, 0⟩ + 3·⟨0, 1⟩ = ⟨2, 3⟩" (2D when is2D, dropping z); an empty slot is left out</summary>
    public static string Combination(float s1, Vector3 v1, float s2, Vector3 v2, bool is2D)
    {
        string left = "";
        if (s1 != 0 && v1 != Vector3.zero)
            left = Term(s1, v1, is2D);
        if (s2 != 0 && v2 != Vector3.zero)
            left += (left.Length > 0 ? " + " : "") + Term(s2, v2, is2D);
        Vector3 r = s1 * v1 + s2 * v2;
        return (left.Length > 0 ? left : "0") + " = " + (is2D ? Vec((Vector2)r) : Vec(r));
    }

    private static string Term(float s, Vector3 v, bool is2D)
    {
        return Num(s) + "·" + (is2D ? Vec((Vector2)v) : Vec(v));
    }
}

/// <summary>
/// Sorts a submitted linear combination s1·v1 + s2·v2 aimed at a target into the mistake types a
/// linear algebra teacher can act on. Checked in this order; the first match wins.
/// </summary>
public static class MathMistakes
{
    public const string Correct = "correct";
    public const string Sign = "sign";                  // right size, a sign flipped (r = -T, or one term's sign)
    public const string Swap = "swap";                  // components or the two scalars swapped
    public const string ParallelPair = "parallel_pair"; // chose two parallel vectors: they can't span the target
    public const string Partial = "partial";            // one term matches the intended solution, the other doesn't
    public const string Scalar = "scalar";              // right direction or right vectors, wrong scalar(s)
    public const string Other = "other";

    /// <param name="intendedVectors">vectors of the intended solution (any parallel card also counts), or null</param>
    /// <param name="intendedTerms">the intended terms x·a and y·b, or null</param>
    public static string Classify(Vector3 target, Vector3 v1, float s1, Vector3 v2, float s2,
                                  Vector3[] intendedVectors = null, Vector3[] intendedTerms = null)
    {
        Vector3 t1 = s1 * v1, t2 = s2 * v2, r = t1 + t2;
        if (Same(r, target))
            return Correct;
        if (Same(r, -target) || Same(t1 - t2, target) || Same(t2 - t1, target))
            return Sign;
        if (IsPermutation(r, target) || Same(s2 * v1 + s1 * v2, target))
            return Swap;
        bool bothUsed = s1 != 0 && s2 != 0 && v1 != Vector3.zero && v2 != Vector3.zero;
        if (bothUsed && Parallel(v1, v2) && !Parallel(v1, target))
            return ParallelPair;
        if (intendedTerms != null)
            foreach (Vector3 term in intendedTerms)
                if (term != Vector3.zero && (Same(t1, term) || Same(t2, term)))
                    return Partial;
        if (r != Vector3.zero && Parallel(r, target) && Vector3.Dot(r, target) > 0)
            return Scalar;
        if (intendedVectors != null && intendedVectors.Length == 2 &&
            ((Parallel(v1, intendedVectors[0]) && Parallel(v2, intendedVectors[1])) ||
             (Parallel(v1, intendedVectors[1]) && Parallel(v2, intendedVectors[0]))))
            return Scalar;
        return Other;
    }

    public static bool Same(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude < 1e-4f;
    }

    public static bool Parallel(Vector3 a, Vector3 b)
    {
        if (a == Vector3.zero || b == Vector3.zero)
            return false;
        return Vector3.Cross(a, b).sqrMagnitude < 1e-4f * a.sqrMagnitude * b.sqrMagnitude;
    }

    private static bool IsPermutation(Vector3 r, Vector3 t)
    {
        if (Same(r, t))
            return false;
        float[] a = { r.x, r.y, r.z }, b = { t.x, t.y, t.z };
        Array.Sort(a);
        Array.Sort(b);
        return Mathf.Abs(a[0] - b[0]) < 1e-3f && Mathf.Abs(a[1] - b[1]) < 1e-3f && Mathf.Abs(a[2] - b[2]) < 1e-3f;
    }
}
