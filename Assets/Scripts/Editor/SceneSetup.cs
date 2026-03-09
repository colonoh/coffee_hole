#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class SceneSetup
{
    [MenuItem("Coffee Hole/Setup MVP Scene")]
    public static void SetupScene()
    {
        // --- Room ---
        CreateFloor();
        CreateWalls();

        // --- Player ---
        GameObject player = CreatePlayer();

        // --- Physics Objects ---
        CreatePhysicsObjects();

        // --- Coffee Cup ---
        CreateCoffeeCup();

        // --- Crosshair UI ---
        CreateCrosshairCanvas();

        Debug.Log("MVP scene setup complete! Enter Play mode to test.");
        Selection.activeGameObject = player;
    }

    private static void CreateFloor()
    {
        if (GameObject.Find("Floor") != null) return;

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0f, -0.5f, 0f);
        floor.transform.localScale = new Vector3(12f, 1f, 12f);
        floor.isStatic = true;

        SetColor(floor, new Color(0.6f, 0.6f, 0.6f));
        Undo.RegisterCreatedObjectUndo(floor, "Create Floor");
    }

    private static void CreateWalls()
    {
        float roomSize = 12f;
        float wallHeight = 4f;
        float wallThickness = 0.5f;
        float halfRoom = roomSize / 2f;
        Color wallColor = new Color(0.75f, 0.75f, 0.75f);

        var wallDefs = new (string name, Vector3 pos, Vector3 scale)[]
        {
            ("Wall_North", new Vector3(0, wallHeight / 2f, halfRoom), new Vector3(roomSize, wallHeight, wallThickness)),
            ("Wall_South", new Vector3(0, wallHeight / 2f, -halfRoom), new Vector3(roomSize, wallHeight, wallThickness)),
            ("Wall_East", new Vector3(halfRoom, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, roomSize)),
            ("Wall_West", new Vector3(-halfRoom, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, roomSize)),
        };

        foreach (var (name, pos, scale) in wallDefs)
        {
            if (GameObject.Find(name) != null) continue;

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.isStatic = true;
            SetColor(wall, wallColor);
            Undo.RegisterCreatedObjectUndo(wall, "Create " + name);
        }
    }

    private static GameObject CreatePlayer()
    {
        // Don't duplicate if already exists
        GameObject existing = GameObject.Find("Player");
        if (existing != null)
        {
            Debug.Log("Player already exists, skipping.");
            return existing;
        }

        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 1f, -4f);

        // CharacterController
        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.center = new Vector3(0f, 0f, 0f);
        cc.radius = 0.4f;

        // PlayerInput — find the InputActionAsset
        var playerInput = player.AddComponent<PlayerInput>();
        string[] guids = AssetDatabase.FindAssets("InputSystem_Actions t:InputActionAsset");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            playerInput.actions = asset;
            playerInput.defaultActionMap = "Player";
        }
        else
        {
            Debug.LogWarning("InputSystem_Actions asset not found! Assign it manually on the PlayerInput component.");
        }

        // FirstPersonController
        player.AddComponent<FirstPersonController>();

        // Camera as child
        // Remove existing main camera so we don't have two
        Camera existingCam = Camera.main;
        if (existingCam != null)
            Undo.DestroyObjectImmediate(existingCam.gameObject);

        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        camObj.transform.SetParent(player.transform, false);
        camObj.transform.localPosition = new Vector3(0f, 0.8f, 0f); // eye height

        Camera cam = camObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.1f;
        camObj.AddComponent<AudioListener>();

        // ObjectGrabber on camera
        camObj.AddComponent<ObjectGrabber>();

        Undo.RegisterCreatedObjectUndo(player, "Create Player");
        return player;
    }

    private static void CreatePhysicsObjects()
    {
        // Skip if objects already placed
        if (GameObject.Find("PhysObj_Cube_1") != null) return;

        var objects = new (string name, PrimitiveType type, Vector3 pos, Vector3 scale, Color color)[]
        {
            ("PhysObj_Cube_1",     PrimitiveType.Cube,     new Vector3(-2f, 0.5f, 1f),   new Vector3(0.8f, 0.8f, 0.8f), new Color(0.9f, 0.3f, 0.3f)),
            ("PhysObj_Cube_2",     PrimitiveType.Cube,     new Vector3(1.5f, 0.5f, 2f),  new Vector3(0.5f, 0.5f, 0.5f), new Color(0.3f, 0.9f, 0.3f)),
            ("PhysObj_Cube_3",     PrimitiveType.Cube,     new Vector3(3f, 0.5f, -1f),   new Vector3(1f, 1f, 1f),       new Color(0.3f, 0.3f, 0.9f)),
            ("PhysObj_Sphere_1",   PrimitiveType.Sphere,   new Vector3(-1f, 0.5f, 3f),   new Vector3(0.6f, 0.6f, 0.6f), new Color(0.9f, 0.9f, 0.2f)),
            ("PhysObj_Sphere_2",   PrimitiveType.Sphere,   new Vector3(2f, 0.5f, -2f),   new Vector3(0.9f, 0.9f, 0.9f), new Color(0.9f, 0.5f, 0.1f)),
            ("PhysObj_Sphere_3",   PrimitiveType.Sphere,   new Vector3(-3f, 0.5f, -2f),  new Vector3(0.4f, 0.4f, 0.4f), new Color(0.8f, 0.2f, 0.8f)),
            ("PhysObj_Cylinder_1", PrimitiveType.Cylinder, new Vector3(0f, 0.5f, 0f),    new Vector3(0.5f, 0.7f, 0.5f), new Color(0.2f, 0.8f, 0.8f)),
            ("PhysObj_Cylinder_2", PrimitiveType.Cylinder, new Vector3(-2.5f, 0.5f, 3f), new Vector3(0.4f, 0.5f, 0.4f), new Color(0.6f, 0.4f, 0.2f)),
        };

        foreach (var (name, type, pos, scale, color) in objects)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = scale;

            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.mass = scale.x * scale.y * scale.z; // rough mass from volume

            SetColor(obj, color);
            Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
        }
    }

    private static void CreateCoffeeCup()
    {
        if (GameObject.Find("CoffeeCup") != null) return;

        GameObject cup = CoffeeCupGenerator.Create(new Vector3(1f, 0.15f, 0f));
        Undo.RegisterCreatedObjectUndo(cup, "Create Coffee Cup");
    }

    private static void CreateCrosshairCanvas()
    {
        if (GameObject.Find("CrosshairCanvas") != null) return;

        GameObject canvasObj = new GameObject("CrosshairCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.AddComponent<CrosshairUI>();

        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Crosshair Canvas");
    }

    private static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        // Create a new material instance so objects don't share the same material
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        renderer.sharedMaterial = mat;
    }
}
#endif
