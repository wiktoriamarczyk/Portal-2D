using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Klasa Lasers odpowiada za zachowanie laserów 
/// </summary>
public class Lasers : MonoBehaviour
{
    /// <summary>
    /// DŸwiêk lasera
    /// </summary>
    [SerializeField] AudioSource laserSound;
    /// <summary>
    /// DŸwiêk w³¹czenia lasera
    /// </summary>
    [SerializeField] AudioSource laserOn;
    /// <summary>
    /// DŸwiêk wy³¹czenia lasera
    /// </summary>
    [SerializeField] AudioSource laserOff;
    /// <summary>
    /// Zdarzenie wywo³ywane, gdy laser trafia w odbiornik
    /// </summary>
    [SerializeField] UnityEvent onReceiverHit;
    /// <summary>
    /// Zdarzenie wywo³ywane, gdy laser znika z odbiornika
    /// </summary>
    [SerializeField] UnityEvent onReceiverReleased;
    /// <summary>
    /// Warstwa obiektów, które mog¹ zablokowaæ laser
    /// </summary>
    [SerializeField] LayerMask layerMask;
    /// <summary>
    /// Domyœlny sprite
    /// </summary>
    public Sprite defaultSprite;
    /// <summary>
    /// Aktywowany sprite (po w³¹czeniu lasera lub po trafieniu w odbiornik)
    /// </summary>
    public Sprite activatedSprite;
    /// <summary>
    /// Wektor z pocz¹tkowym punktem lasera
    /// </summary>
    public Vector3 start;
    /// <summary>
    /// Wektor z odleg³ym punktem na prostej lasera
    /// </summary>
    public Vector3 maxEnd;
    /// <summary>
    /// Wektor z rzeczywistym koñcem lasera (po wykryciu kolizji)
    /// </summary>
    public Vector3 realEnd;
    /// <summary>
    /// Komponent LineRenderer - do rysowania lasera
    /// </summary>
    private LineRenderer lineRenderer;
    /// <summary>
    /// Komponent SpriteRenderer - do zmiany sprite'ów
    /// </summary>
    private SpriteRenderer spriteRenderer;


    /// <summary>
    /// Metoda wywo³ywana przed pierwszym odœwie¿eniem klatki
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();        // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;                     // Ustawienie liczby punktów na 2
        start = transform.position;                         // Pierwszy punkt to pozycja transmitera
        stopLaser();                                        // Wy³¹czenie lasera na pocz¹tku gry
        // Ustawienie w³aœciwego kierunku i zwrotu lasera w zale¿noœci od rotacji transmitera
        if (Math.Round(transform.rotation.z, 1) == -0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y - 100, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        }
        else if (transform.rotation.z == 0.0)
        {
            maxEnd = new Vector3(transform.position.x + 100, transform.position.y, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 3.1)
        {
            maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Metoda powoduj¹ca uruchomienie lasera
    /// </summary>
    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = activatedSprite;
        lineRenderer.enabled = true;    // W³¹czenie linii
        laserOn.Play();
        laserSound.mute = false;
        laserSound.PlayDelayed(laserOn.clip.length);
        laserSound.loop = true;
    }

    /// <summary>
    /// Metoda powoduj¹ca zatrzymanie lasera
    /// </summary>
    public void stopLaser()
    {
        // Zmiana sprite'a na domyœlny
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;   // Wy³¹czenie linii
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
        GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
        laserSound.mute = true;
        laserSound.loop = false;
        if (DoorOut.isActive) GameObject.Find("DoorOut").GetComponent<DoorOut>().CloseDoor();
        laserOff.Play();
    }

    /// <summary>
    /// Metoda wywo³ywana co klatkê
    /// </summary>
    void Update()
    {
        if (lineRenderer.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start); // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
            {
                hit.collider.gameObject.GetComponent<MirrorCube>().startLaser();
            }
            else
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
                if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = activatedSprite;
                    if (!DoorOut.isActive) onReceiverHit.Invoke();
                }
                else
                {
                    if (DoorOut.isActive) onReceiverReleased.Invoke();
                    GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
                    if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                    {
                        Debug.Log("The blue portal was hit by laser");
                        PortalLaser.isBluePortalHit = true;
                    }
                    else if (hit.collider != null && hit.collider.gameObject.tag == "Orange Portal")
                    {
                        Debug.Log("The orange portal was hit by laser");
                        PortalLaser.isOrangePortalHit = true;
                    }
                    else
                    {
                        PortalLaser.isBluePortalHit = false;
                        PortalLaser.isOrangePortalHit = false;
                    }
                }
            }
        }
        else
        {
            PortalLaser.isBluePortalHit = false;
            PortalLaser.isOrangePortalHit = false;
        }
    }

    /// <summary>
    /// Metoda wywo³ywana, gdy laser trafia w odbiornik - zmienia sprite odbiornika na aktywowany
    /// </summary>
    public void ReceiverHit()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = activatedSprite;
    }

    /// <summary>
    /// Metoda wywo³ywana, gdy laser znika z odbiornika - zmienia sprite odbiornika na domyœlny
    /// </summary>
    public void ReceiverReleased()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }
}
