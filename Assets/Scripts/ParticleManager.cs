using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class ParticleManager : MonoBehaviour
{
    [SerializeField]
    private Transform cursor;
    [SerializeField]
    private LayerMask usableLayers;

    [SerializeField]
    private Camera mainCamera;

    private const int BUFFER_STRIDE = 12; // 12 Bytes for a Vector3 (4,4,4)
    private static readonly int VfxBufferProperty = Shader.PropertyToID("Objects");
    [SerializeField] private int bufferInitialCapacity = 32;

    [SerializeField]
    private VisualEffect reactor;
    [SerializeField]
    private VisualEffect magicalLights;
    [SerializeField]
    private VisualEffect manusLogo;

    GraphicsBuffer positionBuffer;

    public List<GameObject> pigeons = new List<GameObject>();
    public List<Vector3> positions = new List<Vector3>();

    private int bufferSize = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // List with data used to fill buffer
        positions = new List<Vector3>(bufferInitialCapacity);
        // Create initial graphics buffer
        EnsureBufferCapacity(ref positionBuffer, bufferInitialCapacity, BUFFER_STRIDE, magicalLights, VfxBufferProperty);

        foreach(GameObject pigeon in pigeons)
        {
            positions.Add(pigeon.transform.position);
        }
        Time.timeScale = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.IsPressed())
        {
            //Vector3 clickPos = -Vector3.one;
            //Vector3 mousePos = Mouse.current.position.ReadValue();
            ////mousePos.z = 10f;

            //Ray ray = mainCamera.ScreenPointToRay(mousePos);
            //RaycastHit hit;
            //if(Physics.Raycast(ray, out hit, 100f, usableLayers))
            //{
            //    clickPos = hit.point;
            //}

            ////clickPos.y += 0.3f;
            //cursor.position = clickPos;

        }
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Spawn();
        }

        //for(int i = 0; i < positions.Count; i++)
        //{
        //    positions[i] = pigeons[i].transform.position;
        //}
        
    }

    private void Spawn()
    {
        manusLogo.SendEvent("OnSpawn");
    }

    void LateUpdate()
    {
        // You can gather data during frame or construct it here
        // data.Clear();
        // data.Add(new Vector3(Random.value, Random.value, Random.value));

        // Set Buffer data, but before that ensure there is enough capacity
        EnsureBufferCapacity(ref positionBuffer, positions.Count, BUFFER_STRIDE, magicalLights, VfxBufferProperty);
        positionBuffer.SetData(positions);
    }

    void OnDestroy()
    {
        ReleaseBuffer(ref positionBuffer);
    }

    private void EnsureBufferCapacity(ref GraphicsBuffer buffer, int capacity, int stride, VisualEffect vfx, int vfxProperty)
    {
        // Reallocate new buffer only when null or capacity is not sufficient
        if(buffer == null || buffer.count < capacity)
        {
            // Buffer memory must be released
            buffer?.Release();
            // Vfx Graph uses structured buffer
            buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, stride);
            // Update buffer referenece
            vfx.SetGraphicsBuffer(vfxProperty, buffer);
        }
    }

    private void ReleaseBuffer(ref GraphicsBuffer buffer)
    {
        // Buffer memory must be released
        buffer?.Release();
        buffer = null;
    }
}