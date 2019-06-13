using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public enum GameStates
    {
        PLAYING,
        WIN,
        LOSE
    }
    
    //Variables del juego
    [Header("Tiempo para jugar:")] public float tiempoInicial;

    private float _tiempo;
    public float Tiempo
    {
        get => _tiempo;
        set => _tiempo = value;
    }

    private int _nivel;
    public int Nivel
    {
        get => _nivel;
        set => _nivel = value;
    }

    private Scene _gameScene;

    public Scene GameScene
    {
        get => _gameScene;
        set => _gameScene = value;
    }

    private GameStates gameState;

    public GameStates GameState
    {
        get => gameState;
        set => gameState = value;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Tiempo = tiempoInicial;
        GameScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        //check game states
        if (GameState.Equals(GameStates.LOSE))
            Tiempo = 0f;
        
        //Check Scenes
        if(GameScene.name.Equals("Titulo"))
            Tiempo = 0f;
    }
}
