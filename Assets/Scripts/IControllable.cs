using UnityEngine;
using System.Collections;

// TODO: Dubtosa interface. Rev de com fer el handle del input
public interface IControllable  {

    void UseMagnet();

    void Move(float x, float y); // Poder canviar-ho per CalculateForceVector com anom

    Vector2 GetForceVector();

    bool IsActive();
}
