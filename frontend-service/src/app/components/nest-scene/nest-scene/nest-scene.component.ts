import { ElementRef, OnInit, ViewChild, Component } from '@angular/core';
import { EngineService } from '../services/engine.service';

@Component({
  selector: 'app-nest-scene',
  templateUrl: './nest-scene.component.html',
  styleUrls: ['./nest-scene.component.scss'],
})
export class NestSceneComponent implements OnInit {

  @ViewChild('rendererCanvas', {static: true})
  public rendererCanvas: ElementRef<HTMLCanvasElement> = {} as ElementRef<HTMLCanvasElement>;

  public constructor(private engineService: EngineService) {
  }

  public ngOnInit(): void {
    this.engineService.createScene(this.rendererCanvas);
    this.engineService.render();
  }
}
