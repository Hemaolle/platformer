var levelToLoad : String;
var Material: Material;
var hoverMaterial: Material;
var soundhover : AudioClip;
var beep : AudioClip;
var QuitButton : boolean = false;

function OnMouseOver(){
renderer.material = hoverMaterial;
}
function OnMouseExit(){
renderer.material = Material;
}
function OnMouseEnter(){
audio.PlayOneShot(soundhover);
}
function OnMouseUp(){
audio.PlayOneShot(beep);
yield new WaitForSeconds(0.35);
if(QuitButton){
Application.Quit();
}
else{
Application.LoadLevel(levelToLoad);
}
}
@script RequireComponent(AudioSource)