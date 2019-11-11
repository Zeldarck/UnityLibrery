using UnityEngine;
using UnityEngine.AI;



public class PlayerController : Character
{

    public Interactable CurrentInteractable { get; private set; }

    public Inventory Inventory { get; private set; }

    public KeyValueData KeyValueData { get; private set; }


    protected override void Awake ()
    {
        base.Awake();

        Inventory = new Inventory();

        KeyValueData = new KeyValueData();

        CameraManager.Instance.Target = gameObject;

    }

    private void Update()
    {
        if (!Freeze)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (CurrentInteractable != null)
                {
                    CurrentInteractable.TryInteract(this);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.TryRelease();
            }
        }

    }

    void FixedUpdate()
    {
        if (!Freeze)
        {
            Move();
        }
    }

    protected override void Move()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        m_movementDirection = new Vector3(horizontal, 0f, vertical);

        base.Move();
    }

    public bool SetInteractable(Interactable a_interactable)
    {
        if(CurrentInteractable == null || CurrentInteractable.gameObject == a_interactable.gameObject)
        {
            CurrentInteractable = a_interactable;
            return true;
        }
        return false;
    }

    public void ResetInteractable()
    {
        CurrentInteractable.GetComponent<InteractableManager>().Release(this);
        CurrentInteractable = null;
    }

}
