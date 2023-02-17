using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
  public GameObject cube;
  public List<GameObject> players;
  public LayerMask layer, LayerMask;
  private Camera _cam;
  private GameObject _cubeSelection;
  private RaycastHit _hit;

  private void Awake()
  {
    _cam = GetComponent<Camera>();  
  }

  private void Update()
  {
    if(Input.GetMouseButtonDown(1) && players.Count > 0)
    {
      Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out RaycastHit agentTarget, 1000f, layer))
        foreach (var el in players)
          el.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(agentTarget.point);
    }

    if (Input.GetMouseButtonDown(0))
    {
      foreach (var el in players)
        el.transform.GetChild(0).gameObject.SetActive(false);
      
      players.Clear();

      Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out _hit, 1000f, layer))
        _cubeSelection = Instantiate(cube, new Vector3(_hit.point.x, 1, _hit.point.z), Quaternion.identity);
    }

    if(_cubeSelection)
    {
      Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out RaycastHit hitDrag, 1000f, layer))
      {
        float xScale = (_hit.point.x - hitDrag.point.x) * -1;
        float zScale = _hit.point.z - hitDrag.point.z;

        if(xScale < 0.0f && zScale < 0.0f)
          _cubeSelection.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else if(xScale < 0.0f)
          _cubeSelection.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        else if(zScale < 0.0f)
          _cubeSelection.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
        else
          _cubeSelection.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        _cubeSelection.transform.localScale = new Vector3(Mathf.Abs(xScale), 1, Mathf.Abs(zScale));
      }
    }

    if(Input.GetMouseButtonUp(0) && _cubeSelection)
    {
      RaycastHit[] hits =  Physics.BoxCastAll(
        _cubeSelection.transform.position,
        _cubeSelection.transform.localScale,
        Vector3.up,
        Quaternion.identity,
        0,
        LayerMask);

      foreach (var el in hits)
      {
        if(el.collider.CompareTag("Enemy")) continue;

        players.Add(el.transform.gameObject);
        el.transform.GetChild(0).gameObject.SetActive(true);
      }

      Destroy(_cubeSelection);
    }

  }
}
