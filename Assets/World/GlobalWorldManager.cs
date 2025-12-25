using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using Cards.Environments;
using Characters;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Utils;

public class GlobalWorldManager : MonoBehaviour
{
    public static GlobalWorldManager Instance { get; private set; }
    public static event Action OnAfterMove;
    public float RawTransitionTime { get; private set; }
    public float CurvedTransitionTime => transitionCurve.Evaluate(RawTransitionTime);
    public AnimationCurve transitionCurve;
    public CardEnv CurrentEnvironment;
    public PlayerAgent playerAgent;
    public float newDomainOffset;
    public GameObject sphereEffect;
    
    private bool _transitioning;
    public string TransitionTag { private set; get; }
    private GameObject[] _newObjects, _oldObjects;
    private Scene _oldScene, _newScene;
    private float _dissolveRad;
    private float _transitionSpeed;
    private bool _flipped;
    private Vector3 _startPosition;
    private string _startName;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerAgent = FindAnyObjectByType<PlayerAgent>();
    }
    void Start()
    {
        RenderPipelineManager.beginContextRendering += Move;
        RenderPipelineManager.endContextRendering += MoveBack;
        
        var mpb2 = new MaterialPropertyBlock();
        mpb2.SetFloat("_NoDissolve", 1);
        
        foreach (var rend in PlayerSingleton.Instance.GetComponentsInChildren<Renderer>())
        {
            rend.SetPropertyBlock(mpb2);
        }
        
    }
    public void Transition(string newSceneName, Vector3 startPosition, string startName, string tags)
    {
        if (_transitioning) return;
        _startPosition = startPosition;
        _startName = startName;
        TransitionTag = tags;
        _ = LoadSceneAsyncWithCallback(newSceneName);
    }
    
    public async Task LoadSceneAsyncWithCallback(string sceneName)
    {
        _oldScene = SceneManager.GetActiveScene();
        RawTransitionTime = 0;
        
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!op.isDone)
            await Task.Yield();
        
        _newScene = SceneManager.GetSceneByName(sceneName);
        //op.allowSceneActivation = false;
        _newObjects = _newScene.GetRootGameObjects();
        _oldObjects = _oldScene.GetRootGameObjects();
        _flipped = false;
        
        var mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_Flipped", 1);
        
        foreach (GameObject go in _newObjects)
        {
            go.transform.position += Vector3.right * newDomainOffset;
            foreach (var rend in go.GetComponentsInChildren<Renderer>())
            {
                rend.SetPropertyBlock(mpb);
            }

            foreach (var rend in go.GetComponentsInChildren<IRenderable>())
            {
                rend.SetPropertyBlock(mpb);
            }
        }
        
        mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_Flipped", 0);
        
        foreach (GameObject go in _oldObjects)
        {
            foreach (var rend in go.GetComponentsInChildren<Renderer>())
            {
                rend.SetPropertyBlock(mpb);
            }
            foreach (var rend in go.GetComponentsInChildren<IRenderable>())
            {
                rend.SetPropertyBlock(mpb);
            }
        }
        Debug.Log("Scene loaded!");
    }

    public void Load(CardEnv env)
    {
        if (CurrentEnvironment == null)
        {
            CurrentEnvironment = env;
            env.Initialize(playerAgent);
        }
        else
        {
            CurrentEnvironment.Destroy();
            _dissolveRad = env.environmentIntroRad;
            float introTime = env.environmentIntroTime;
            _transitioning = true;
            float flipTime = Mathf.Min(introTime, env.environmentExitTime);
            float stopTime = introTime;
            _transitionSpeed = 1 / env.environmentIntroTime;
            Shader.SetGlobalVector("_DissolveCenter", _startPosition);
            Delay.Call(0.01f, () =>
            {
                var center = env.GetEnvCenter(_startName);
                if (center)
                {
                    var disp = _startPosition - (center.position - Vector3.right * newDomainOffset);
                    foreach (var go in _newObjects)
                    {
                        if (go) go.transform.position += disp;
                    }
                }
            });
            
            sphereEffect.transform.position = _startPosition;
            sphereEffect.transform.localScale = Vector3.zero;
            
            Delay.Call(flipTime, () =>
            {
                foreach (var go in _newObjects)
                {
                    if (go) go.transform.position -= Vector3.right * newDomainOffset;
                }
                foreach (var go in _oldObjects)
                {
                    if (go) go.transform.position += Vector3.right * newDomainOffset;
                }

                _flipped = true;
            });
            
            Delay.Call(stopTime, () =>
            {
                CurrentEnvironment = env;
                env.Initialize(playerAgent);
                if (_transitioning) EndTransition();
            });
        }
    }

    public void EndTransition()
    {
        _transitioning = false;
        RawTransitionTime = -1;
        Shader.SetGlobalVector("_DissolveData", new Vector4(-5, 0, 0, 0));
        
        var mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_Flipped", 0);
        
        foreach (GameObject go in _newScene.GetRootGameObjects())
        {
            foreach (var rend in go.GetComponentsInChildren<Renderer>())
            {
                rend.SetPropertyBlock(mpb);
            }

            foreach (var rend in go.GetComponentsInChildren<IRenderable>())
            {
                rend.SetPropertyBlock(mpb);
            }
        }
        
        _ = SceneManager.UnloadSceneAsync(_oldScene);
    }

    // Update is called once per frame
    void Update()
    {
        if (_transitioning) RawTransitionTime += Time.deltaTime * _transitionSpeed;
    }

    void OnDestroy()
    {
        RenderPipelineManager.beginContextRendering -= Move;
        RenderPipelineManager.endContextRendering -= MoveBack;
    }

    public Vector4 shaders, shaderColor;
    
    [ContextMenu("Set Shader Props")]
    void Dissolve()
    {
        // Radius
        // Strength
        // Scale
        // Flipped
        // Color: RGB, scale
        Shader.SetGlobalVector("_DissolveCenter", Vector4.zero);
        Shader.SetGlobalVector("_DissolveData", shaders);
        Shader.SetGlobalVector("_DissolveColor", shaderColor);
        Shader.SetGlobalFloat("_Flipped", shaders[3]);
    }
    void Move(ScriptableRenderContext context, List<Camera> camera)
    {
        if (_transitioning)
        {
            
            var r = transitionCurve.Evaluate(RawTransitionTime) * _dissolveRad;
            sphereEffect.transform.localScale = r * Vector3.one * 2;
            
            foreach (var go in _flipped? _oldObjects : _newObjects)
            {
                if (go) go.transform.position -= Vector3.right * newDomainOffset;
            }
            Shader.SetGlobalVector("_DissolveData", new Vector4(r, shaders[1], shaders[2], 0));
        }
        OnAfterMove?.Invoke();
    }

    void MoveBack(ScriptableRenderContext context, List<Camera> camera)
    {
        if (_transitioning)
        {
            foreach (var go in _flipped? _oldObjects : _newObjects)
            {
                if (go) go.transform.position += Vector3.right * newDomainOffset;
            }
        }
    }

    private void OnDisable()
    {
        Dissolve();
    }
}

public interface IRenderable
{
    public void SetPropertyBlock(MaterialPropertyBlock block);
}