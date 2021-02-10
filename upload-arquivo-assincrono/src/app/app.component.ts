import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  @ViewChild('inputUploadDeArquivo') inputUploadDeArquivo: any;

  formulario!: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private httpClient: HttpClient) { }

  ngOnInit(): void {
    this.formulario = this.formBuilder.group({
      uploadDeArquivo: this.formBuilder.control('')
    });
  }


  realizarUpload = () => {
    debugger;
    if (this.inputUploadDeArquivo && this.inputUploadDeArquivo.nativeElement && this.inputUploadDeArquivo.nativeElement.files.length) {
      const arquivo = this.inputUploadDeArquivo.nativeElement.files[0];
      const formData = new FormData()
      formData.append('file', arquivo, arquivo.name);

      // Realizar chamada do serviço
      const urlEndpoint = 'http://localhost:62359/upload/excel';
      this.httpClient.post(urlEndpoint, formData)
        .subscribe(event => alert('Upload completo!'),
        (erro) => alert(erro));

    } else {
      alert('Selecione ao menos um arquivo.');
      return;
    }
  }

}
