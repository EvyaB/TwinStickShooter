using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class Explodable : MonoBehaviour
{
    public System.Action<List<GameObject>> OnFragmentsGenerated;

    public bool allowRuntimeFragmentation = false;
    public int extraPoints = 0;
    public int subshatterSteps = 0;

    public string fragmentLayer = "Default";
    public string sortingLayerName = "Default";
    public int orderInLayer = 0;
    public float fragmentLifetime = 0;
    public float fragmentsGravityScale = 1f;
    public SpriteRenderer spriteRenderer;

    public enum ShatterType
    {
        Triangle,
        Voronoi
    };

    public ShatterType shatterType;
    public List<GameObject> fragments = new List<GameObject>();
    private List<List<Vector2>> polygons = new List<List<Vector2>>();

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Creates fragments if necessary and destroys original gameobject
    /// </summary>
    public void explode()
    {
        if (spriteRenderer == null) { Debug.LogError(name + " Explodable object has no SpriteRenderer defined, cannot explode"); return; }

        //if fragments were not created before runtime then create them now
        if (fragments.Count == 0 && allowRuntimeFragmentation)
        {
            generateFragments();
        }
        //otherwise activate existing fragments
        else
        {
            foreach (GameObject frag in fragments)
            {
                var mRend = frag.GetComponent<MeshRenderer>();
                if (mRend.sharedMaterial == null)
                {
                    var sRend = spriteRenderer;
                    mRend.sharedMaterial = sRend.sharedMaterial;
                    mRend.sharedMaterial.SetTexture("_MainTex", sRend.sprite.texture);
                }
                frag.SetActive(true);
            }
        }
        //if fragments exist destroy the original
        if (fragments.Count > 0)
        {
            var renderer = spriteRenderer;
            var colider = gameObject.GetComponent<Collider2D>();

            renderer.enabled = false;
            colider.enabled = false;
        }
    }
    /// <summary>
    /// Creates fragments and then disables them
    /// </summary>
    public void fragmentInEditor()
    {
        if (fragments.Count > 0)
        {
            deleteFragments();
        }
        generateFragments();
        setPolygonsForDrawing();
        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }
    public void deleteFragments()
    {
        foreach (GameObject frag in fragments)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(frag);
            }
            else
            {
                Destroy(frag);
            }
        }
        fragments.Clear();
        polygons.Clear();
    }

    public void fragmentInEditor_meshSave()
    {
        if (fragments.Count > 0)
        {
            deleteFragments();
        }
        generateFragments(true);
        setPolygonsForDrawing();

        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }

    /// <summary>
    /// Turns Gameobject into multiple fragments
    /// </summary>
    private void generateFragments(bool meshSaved = false)
    {
        fragments = new List<GameObject>();

        Material mat = null;
        if (meshSaved)
        {
            mat = SpriteExploder.createFragmentMaterial(spriteRenderer);
            Directory.CreateDirectory("Assets/FragmentMaterials");
            AssetDatabase.CreateAsset(mat, "Assets/FragmentMaterials/" + transform.name + "_fragments" + ".mat");
        }

        switch (shatterType)
        {
            case ShatterType.Triangle:
                fragments = SpriteExploder.GenerateTriangularPieces(gameObject, spriteRenderer, extraPoints, subshatterSteps, mat, meshSaved);
                break;
            case ShatterType.Voronoi:
                fragments = SpriteExploder.GenerateVoronoiPieces(gameObject, spriteRenderer, extraPoints, subshatterSteps, mat, meshSaved);
                break;
            default:
                Debug.Log("invalid choice");
                break;
        }

        
        for (int i = 0; i < fragments.Count; i ++)
        {
            if (fragments[i] != null)
            {
                fragments[i].layer = LayerMask.NameToLayer(fragmentLayer);
                fragments[i].GetComponent<Renderer>().sortingLayerName = sortingLayerName;
                fragments[i].GetComponent<Renderer>().sortingOrder = orderInLayer;

                /// prefab mesh save        
                if (meshSaved)
                {
                    if (!string.IsNullOrEmpty("Assets/FragmentMesh"))
                    {
                        Directory.CreateDirectory("Assets/FragmentMesh");
                    }

                    var mesh = fragments[i].GetComponent<MeshFilter>().sharedMesh;
                    AssetDatabase.CreateAsset(mesh, "Assets/FragmentMesh/" + transform.name + "_" + i + ".asset");
                }

                // if lifetime set - add DestroyObjectDelay component to destroy pieces with delay
                if (fragmentLifetime > 0)
                {
                    fragments[i].AddComponent<DestroyObjectDelay>().delay = fragmentLifetime;
                }

                fragments[i].GetComponent<Rigidbody2D>().gravityScale = fragmentsGravityScale;
            }
        }

        AssetDatabase.SaveAssets();

        foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
        {
            if (addon.enabled)
            {
                addon.OnFragmentsGenerated(fragments);
            }
        }
    }
    private void setPolygonsForDrawing()
    {
        polygons.Clear();
        List<Vector2> polygon;

        foreach (GameObject frag in fragments)
        {
            polygon = new List<Vector2>();
            foreach (Vector2 point in frag.GetComponent<PolygonCollider2D>().points)
            {
                Vector2 offset = rotateAroundPivot((Vector2)frag.transform.position, (Vector2)transform.position, Quaternion.Inverse(transform.rotation)) - (Vector2)transform.position;
                offset.x /= transform.localScale.x;
                offset.y /= transform.localScale.y;
                polygon.Add(point + offset);
            }
            polygons.Add(polygon);
        }
    }
    private Vector2 rotateAroundPivot(Vector2 point, Vector2 pivot, Quaternion angle)
    {
        Vector2 dir = point - pivot;
        dir = angle * dir;
        point = dir + pivot;
        return point;
    }

    void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (polygons.Count == 0 && fragments.Count != 0)
            {
                setPolygonsForDrawing();
            }

            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector2 offset = (Vector2)transform.position * 0;
            foreach (List<Vector2> polygon in polygons)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    if (i + 1 == polygon.Count)
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[0] + offset);
                    }
                    else
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[i + 1] + offset);
                    }
                }
            }
        }
    }
}
