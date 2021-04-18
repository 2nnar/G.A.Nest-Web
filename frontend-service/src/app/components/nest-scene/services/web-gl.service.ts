import { Injectable } from '@angular/core';
import fragmentShaderSrc from '../../../../assets/fragment-shader.glsl';
import vertexShaderSrc from '../../../../assets/vertex-shader.glsl';

@Injectable()
export class WebGLService {
  private renderingContext: RenderingContext | null = null;
  private get gl(): WebGLRenderingContext {
    return this.renderingContext as WebGLRenderingContext;
  }

  constructor() {}

  initialiseWebGLContext(canvas: HTMLCanvasElement): void {
    // Try to grab the standard context. If it fails, fallback to experimental.
    this.renderingContext = canvas.getContext('webgl') || canvas.getContext('experimental-webgl');
    // If we don't have a GL context, give up now... only continue if WebGL is available and working...
    if (!this.gl) {
        alert('Unable to initialize WebGL. Your browser may not support it.');
        return;
    }
    // *** set width, height and initialise the webgl canvas ***
    this.setWebGLCanvasDimensions(canvas);
    this.initialiseWebGLCanvas();

    // initialise shaders into WebGL
    let shaderProgram = this.initializeShaders();
  }

  setWebGLCanvasDimensions(canvas: HTMLCanvasElement): void {
    // set width and height based on canvas width and height - good practice to use clientWidth and clientHeight
    this.gl.canvas.width = canvas.clientWidth;
    this.gl.canvas.height = canvas.clientHeight;
  }

  initialiseWebGLCanvas(): void {
    // Set clear colour to black, fully opaque
    this.gl.clearColor(0.0, 0.0, 0.0, 1.0);
    // Enable depth testing
    this.gl.enable(this.gl.DEPTH_TEST);
    // Near things obscure far things
    this.gl.depthFunc(this.gl.LEQUAL);
    // Clear the colour as well as the depth buffer.
    // tslint:disable-next-line: no-bitwise
    this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
  }

  private determineShaderType(shaderMimeType: string): number {
    if (shaderMimeType) {
      if (shaderMimeType === 'x-shader/x-vertex') {
        return this.gl.VERTEX_SHADER;
      } else if (shaderMimeType === 'x-shader/x-fragment') {
        return this.gl.FRAGMENT_SHADER;
      } else {
        console.log('Error: could not determine the shader type');
      }
    }
    return -1;
  }

  private loadShader(shaderSource: string, shaderType: string): WebGLShader | null {
    const shaderTypeAsNumber = this.determineShaderType(shaderType);
    if (shaderTypeAsNumber < 0) {
      return null;
    }
    // Create the gl shader
    const glShader = this.gl.createShader(shaderTypeAsNumber);

    if (!glShader)
      return null;

    // Load the source into the shader
    this.gl.shaderSource(glShader, shaderSource);

    // Compile the shaders
    this.gl.compileShader(glShader);

    // Check the compile status
    const compiledShader = this.gl.getShaderParameter(
      glShader,
      this.gl.COMPILE_STATUS
    );
    return this.checkCompiledShader(compiledShader) ? glShader : null;
  }

  private checkCompiledShader(compiledShader: any): boolean {
    if (!compiledShader) {
      // shader failed to compile, get the last error
      const lastError = this.gl.getShaderInfoLog(compiledShader);
      console.log("couldn't compile the shader due to: " + lastError);
      this.gl.deleteShader(compiledShader);
      return false;
    }
    return true;
  }

  initializeShaders(): WebGLProgram | null {
    // 1. Create the shader program
    let shaderProgram = this.gl.createProgram();

    if (!shaderProgram)
      return null;

    // 2. compile the shaders
    const compiledShaders = [];
    let fragmentShader = this.loadShader(
      fragmentShaderSrc,
      'x-shader/x-fragment'
    );
    let vertexShader = this.loadShader(
      vertexShaderSrc,
      'x-shader/x-vertex'
    );
    compiledShaders.push(fragmentShader);
    compiledShaders.push(vertexShader);

    // 3. attach the shaders to the shader program using our WebGLContext
    if (compiledShaders && compiledShaders.length > 0) {
      for (let i = 0; i < compiledShaders.length; i++) {
        const compiledShader = compiledShaders[i];
        if (compiledShader) {
          this.gl.attachShader(shaderProgram, compiledShader);
        }
      }
    }

    // 4. link the shader program to our gl context
    this.gl.linkProgram(shaderProgram);

    // 5. check if everything went ok
    if (!this.gl.getProgramParameter(shaderProgram, this.gl.LINK_STATUS)) {
      console.log(
        'Unable to initialize the shader program: ' +
          this.gl.getProgramInfoLog(shaderProgram)
      );
    }

    // 6. return shader
    return shaderProgram;
  }
}
