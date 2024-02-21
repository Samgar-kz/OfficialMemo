import { ClassicEditor } from "@ckeditor/ckeditor5-editor-classic";
import { Alignment } from "@ckeditor/ckeditor5-alignment";
import { Autoformat } from "@ckeditor/ckeditor5-autoformat";
import {
  Bold,
  Code,
  Italic,
  Strikethrough,
  Subscript,
  Superscript,
  Underline,
} from "@ckeditor/ckeditor5-basic-styles";
import { BlockQuote } from "@ckeditor/ckeditor5-block-quote";
import { CKBox } from "@ckeditor/ckeditor5-ckbox";
import { CloudServices } from "@ckeditor/ckeditor5-cloud-services";
import { CodeBlock } from "@ckeditor/ckeditor5-code-block";
import { TableOfContents } from "@ckeditor/ckeditor5-document-outline";
import { Essentials } from "@ckeditor/ckeditor5-essentials";
import { ExportPdf } from "@ckeditor/ckeditor5-export-pdf";
import { ExportWord } from "@ckeditor/ckeditor5-export-word";
import { FindAndReplace } from "@ckeditor/ckeditor5-find-and-replace";
import { Font } from "@ckeditor/ckeditor5-font";
import { GeneralHtmlSupport } from "@ckeditor/ckeditor5-html-support";
import { Heading } from "@ckeditor/ckeditor5-heading";
import { Highlight } from "@ckeditor/ckeditor5-highlight";
import { HorizontalLine } from "@ckeditor/ckeditor5-horizontal-line";
import {
  AutoImage,
  Image,
  ImageCaption,
  ImageInsert,
  ImageResize,
  ImageStyle,
  ImageToolbar,
  ImageUpload,
  PictureEditing,
} from "@ckeditor/ckeditor5-image";
import { Indent, IndentBlock } from "@ckeditor/ckeditor5-indent";
import { AutoLink, Link, LinkImage } from "@ckeditor/ckeditor5-link";
import {
  DocumentList,
  DocumentListProperties,
  TodoDocumentList,
} from "@ckeditor/ckeditor5-list";
import { MediaEmbed } from "@ckeditor/ckeditor5-media-embed";
import { Mention } from "@ckeditor/ckeditor5-mention";
import { PageBreak } from "@ckeditor/ckeditor5-page-break";
import { Paragraph } from "@ckeditor/ckeditor5-paragraph";
import { PasteFromOffice } from "@ckeditor/ckeditor5-paste-from-office";
import { RemoveFormat } from "@ckeditor/ckeditor5-remove-format";
import { ShowBlocks } from "@ckeditor/ckeditor5-show-blocks";
import { SourceEditing } from "@ckeditor/ckeditor5-source-editing";
import {
  SpecialCharacters,
  SpecialCharactersEssentials,
} from "@ckeditor/ckeditor5-special-characters";
import { Style } from "@ckeditor/ckeditor5-style";
import {
  Table,
  TableCaption,
  TableCellProperties,
  TableColumnResize,
  TableProperties,
  TableToolbar,
} from "@ckeditor/ckeditor5-table";
import { TextTransformation } from "@ckeditor/ckeditor5-typing";
import { WordCount } from "@ckeditor/ckeditor5-word-count";
import "@ckeditor/ckeditor5-build-classic/build/translations/ru";

class Editor extends ClassicEditor {
  public static override builtinPlugins = [
    Autoformat,
    BlockQuote,
    Bold,
    Heading,
    Image,
    ImageCaption,
    ImageStyle,
    ImageToolbar,
    Indent,
    Italic,
    Link,
    DocumentList,
    MediaEmbed,
    Paragraph,
    Table,
    TableToolbar,
    Alignment,
    AutoImage,
    AutoLink,
    CKBox,
    CloudServices,
    Code,
    CodeBlock,
    Essentials,
    ExportPdf,
    ExportWord,
    FindAndReplace,
    Font,
    Highlight,
    HorizontalLine,
    ImageInsert,
    ImageResize,
    ImageUpload,
    IndentBlock,
    GeneralHtmlSupport,
    LinkImage,
    DocumentListProperties,
    TodoDocumentList,
    Mention,
    PageBreak,
    PasteFromOffice,
    PictureEditing,
    RemoveFormat,
    ShowBlocks,
    SourceEditing,
    SpecialCharacters,
    SpecialCharactersEssentials,
    Style,
    Strikethrough,
    Subscript,
    Superscript,
    TableCaption,
    TableCellProperties,
    TableColumnResize,
    TableProperties,
    TextTransformation,
    Underline,
    WordCount,
  ];
  // Editor configuration.
  public static override defaultConfig = {
    language: "ru",
    height: 500,
    toolbar: {
      items: [
        "undo",
        "redo",
        "|",
        "heading",
        "|",
        "style",
        "|",
        "fontSize",
        "fontFamily",
        "fontColor",
        "fontBackgroundColor",
        "-",
        "bold",
        "italic",
        "underline",
        {
          label: "Formatting",
          icon: "text",
          items: [
            "strikethrough",
            "subscript",
            "superscript",
            "code",
            "horizontalLine",
            "|",
            "removeFormat",
          ],
        },
        "specialCharacters",
        "pageBreak",
        "|",
        "link",
        "insertImage",
        "insertTable",
        "highlight",
        "blockQuote",
        "|",
        "alignment",
        "|",
        "bulletedList",
        "numberedList",
        "todoList",
        "outdent",
        "indent",
      ],
      shouldNotGroupWhenFull: true,
    },
    // htmlSupport: {
    //   allow: [
    //     {
    //       name: /^.*$/,
    //       styles: true,
    //       attributes: true,
    //       classes: true,
    //     },
    //   ],
    // },
    style: {
      definitions: [
        {
          name: "Article category",
          element: "h3",
          classes: ["category"],
        },
        {
          name: "Title",
          element: "h2",
          classes: ["document-title"],
        },
        {
          name: "Subtitle",
          element: "h3",
          classes: ["document-subtitle"],
        },
        {
          name: "Info box",
          element: "p",
          classes: ["info-box"],
        },
        {
          name: "Side quote",
          element: "blockquote",
          classes: ["side-quote"],
        },
        {
          name: "Marker",
          element: "span",
          classes: ["marker"],
        },
        {
          name: "Spoiler",
          element: "span",
          classes: ["spoiler"],
        },
        {
          name: "Code (dark)",
          element: "pre",
          classes: ["fancy-code", "fancy-code-dark"],
        },
        {
          name: "Code (bright)",
          element: "pre",
          classes: ["fancy-code", "fancy-code-bright"],
        },
      ],
    },
    fontFamily: {
      supportAllValues: true,
    },
    fontSize: {
      options: [10, 12, 14, "default", 18, 20, 22],
      supportAllValues: true,
    },
    image: {
      resizeOptions: [
        {
          name: "resizeImage:original",
          label: "Original",
          value: null,
        },
        {
          name: "resizeImage:50",
          label: "50%",
          value: "50",
        },
        {
          name: "resizeImage:75",
          label: "75%",
          value: "75",
        },
      ],
      toolbar: [
        "imageTextAlternative",
        "toggleImageCaption",
        "|",
        "imageStyle:inline",
        "imageStyle:wrapText",
        "imageStyle:breakText",
        "imageStyle:side",
        "|",
        "resizeImage",
      ],
      insert: {
        integrations: ["insertImageViaUrl"],
      },
    },
    list: {
      properties: {
        styles: true,
        startIndex: true,
        reversed: true,
      },
    },
    // link: {
    //   decorators: {
    //     addTargetToExternalLinks: true,
    //     defaultProtocol: "https://",
    //     toggleDownloadable: {
    //       mode: "manual",
    //       label: "Downloadable",
    //       attributes: {
    //         download: "file",
    //       },
    //     },
    //   },
    // },
    mention: {
      feeds: [
        {
          marker: "@",
          feed: [
            "@apple",
            "@bears",
            "@brownie",
            "@cake",
            "@cake",
            "@candy",
            "@canes",
            "@chocolate",
            "@cookie",
            "@cotton",
            "@cream",
            "@cupcake",
            "@danish",
            "@donut",
            "@dragée",
            "@fruitcake",
            "@gingerbread",
            "@gummi",
            "@ice",
            "@jelly-o",
            "@liquorice",
            "@macaroon",
            "@marzipan",
            "@oat",
            "@pie",
            "@plum",
            "@pudding",
            "@sesame",
            "@snaps",
            "@soufflé",
            "@sugar",
            "@sweet",
            "@topping",
            "@wafer",
          ],
          minimumCharacters: 1,
        },
      ],
    },
    // template: {
    //   definitions: [
    //     {
    //       title: "The title of the template",
    //       description: "A longer description of the template",
    //       data: "<p>Data inserted into the content</p>",
    //     },
    //     {
    //       // ...
    //     },
    //     // More template definitions.
    //     // ...
    //   ],
    // },
    placeholder: "Текст СЗ",
    table: {
      contentToolbar: [
        "tableColumn",
        "tableRow",
        "mergeTableCells",
        "tableProperties",
        "tableCellProperties",
        "toggleTableCaption",
      ],
    },
  };
}

export default Editor;
