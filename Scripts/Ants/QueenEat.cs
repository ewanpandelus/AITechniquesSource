using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QueenEat : MonoBehaviour
{
    [SerializeField] private Queen queen;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            if (!collision.GetComponent<Food>().GetEaten())
            {
                if(SceneManager.GetActiveScene().name == "Cave")
                {
                    Ant _ant = collision.gameObject.transform.parent.GetComponent<Ant>();
                    collision.gameObject.transform.parent = null;
                    queen.Eat();
                    collision.GetComponent<Food>().SetEaten(true);
                    collision.GetComponent<Food>().EatAnimation(_ant);
                }
                else
                {
                    RBSAnt _ant = collision.gameObject.transform.parent.GetComponent<RBSAnt>();
                    _ant.SetHeldObject(null);
                    collision.gameObject.transform.parent = null;
                    queen.Eat();
                    collision.GetComponent<Food>().SetEaten(true);
                    collision.GetComponent<Food>().EatAnimation(_ant);
                }
            
            }
        }
    }
}
