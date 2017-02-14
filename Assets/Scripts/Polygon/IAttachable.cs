using UnityEngine;
using System.Collections;

public interface IAttachable  {

    //AttachPoint GetAttachPointClosestTo(Vector3 point);

    void AttachTo(Polygon target);

    void Detach();
}
