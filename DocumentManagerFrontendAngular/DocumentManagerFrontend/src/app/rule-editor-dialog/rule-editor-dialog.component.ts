// rule-editor-dialog.component.ts
import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef, MatDialogModule} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {Rule} from '../dataModel/rule';
import {RuleNodeComponent} from './rule-node/rule-node.component';
import {signal} from '@angular/core';

@Component({
  selector: 'app-rule-editor-dialog',
  templateUrl: './rule-editor-dialog.component.html',
  styleUrls: ['./rule-editor-dialog.component.scss'],
  imports: [MatDialogModule, MatButton, RuleNodeComponent]
})
export class RuleEditorDialogComponent {
  rule = signal<Rule| undefined>(undefined);

  constructor(
    private dialogRef: MatDialogRef<RuleEditorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { rule: Rule | undefined }
  ) {
    this.rule.set(this.data.rule);
  }

  save() {
    this.dialogRef.close(this.rule());
  }

  cancel() {
    this.dialogRef.close(undefined);
  }
}
