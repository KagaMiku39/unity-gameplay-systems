using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WorldPickupSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";

    [MenuItem("Tools/Inventory Study/Build Pickup Demo")]
    public static void BuildPickupDemo()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        CreateGround();
        CreatePlayer();
        CreatePickup(
            "ApplePickup",
            1,
            5,
            true,
            new Vector3(-2f, 0.5f, 0f)
        );
        CreatePickup(
            "SeedPickup",
            2,
            20,
            false,
            new Vector3(0f, 0.5f, 0f)
        );
        CreatePickup(
            "HoePickup",
            3,
            1,
            false,
            new Vector3(2f, 0.5f, 0f)
        );
        ConfigureCamera();

        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"保存场景失败：{ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log(
            "Pickup demo created. Walk into Apple for automatic pickup; " +
            "face Seed or Hoe and press F."
        );
    }

    private static void CreateGround()
    {
        GameObject ground = GameObject.Find("PickupGround");

        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "PickupGround";
        }

        ground.transform.position = new Vector3(0f, -0.25f, 0f);
        ground.transform.localScale = new Vector3(12f, 0.5f, 10f);
    }

    private static void CreatePlayer()
    {
        GameObject player = GameObject.Find("Player");

        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
        }

        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, -3f);

        Rigidbody body = player.GetComponent<Rigidbody>();

        if (body == null)
        {
            body = player.AddComponent<Rigidbody>();
        }

        body.useGravity = false;
        body.isKinematic = true;
        body.constraints = RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionY;

        if (player.GetComponent<SimplePlayerMovement>() == null)
        {
            player.AddComponent<SimplePlayerMovement>();
        }
    }

    private static void CreatePickup(
        string objectName,
        int itemId,
        int amount,
        bool collectOnTrigger,
        Vector3 position
    )
    {
        GameObject pickup = GameObject.Find(objectName);

        if (pickup == null)
        {
            pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = objectName;
        }

        pickup.transform.position = position;
        pickup.transform.localScale = Vector3.one * 0.7f;

        SphereCollider trigger = pickup.GetComponent<SphereCollider>();
        trigger.isTrigger = true;

        Rigidbody body = pickup.GetComponent<Rigidbody>();

        if (body == null)
        {
            body = pickup.AddComponent<Rigidbody>();
        }

        body.useGravity = false;
        body.isKinematic = true;

        WorldItemPickup worldItemPickup =
            pickup.GetComponent<WorldItemPickup>();

        if (worldItemPickup == null)
        {
            worldItemPickup = pickup.AddComponent<WorldItemPickup>();
        }

        SerializedObject pickupObject =
            new SerializedObject(worldItemPickup);
        pickupObject.FindProperty("itemId").intValue = itemId;
        pickupObject.FindProperty("amount").intValue = amount;
        pickupObject.FindProperty("collectOnTrigger").boolValue =
            collectOnTrigger;
        pickupObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(worldItemPickup);
    }

    private static void ConfigureCamera()
    {
        Camera camera = Camera.main;

        if (camera == null)
        {
            return;
        }

        camera.transform.position = new Vector3(0f, 8f, -8f);
        camera.transform.rotation = Quaternion.Euler(38f, 0f, 0f);
    }
}
